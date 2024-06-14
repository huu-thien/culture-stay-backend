using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.HostPayment.QueryParameters;
using CultureStay.Application.ViewModels.HostPayment.Response;
using CultureStay.Application.ViewModels.HostPayment.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public class HostPaymentService (IRepositoryBase<HostPayment> hostPaymentRepository, 
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser) : BaseService(unitOfWork, mapper, currentUser), IHostPaymentService
{
    public async Task<PaginatedList<HostPaymentResponse>> GetPaymentsAsync(HostPaymentQueryParameters hqp)
    {
        var spec = new GetHostPaymentsSpecification(hqp);
        var (hostPayments,totalCount) = await hostPaymentRepository.FindWithTotalCountAsync(spec);
        var hostPaymentsDto = Mapper.Map<List<HostPaymentResponse>>(hostPayments);
        return new PaginatedList<HostPaymentResponse>(hostPaymentsDto, totalCount, hqp.PageIndex, hqp.PageSize);
    }

    public async Task<HostPaymentResponse> GetPaymentAsync(int hostPaymentId)
    {
        var spec = new GetHostPaymentSpecification(hostPaymentId);
        var hostPayment = await hostPaymentRepository.FindOneAsync(spec)
                          ?? throw new EntityNotFoundException(nameof(HostPayment), hostPaymentId.ToString());
        return Mapper.Map<HostPaymentResponse>(hostPayment);
    }

    public async Task Pay(int hostPaymentId)
    {
        var hostPayment = await hostPaymentRepository.GetByIdAsync(hostPaymentId)
                          ?? throw new EntityNotFoundException(nameof(HostPayment), hostPaymentId.ToString());
        hostPayment.Status = HostPaymentStatus.Paid;
        hostPaymentRepository.Update(hostPayment);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<PaginatedList<HostPaymentResponse>> GetPaymentsByHostIdAsync(int hostId, HostPaymentQueryParameters hqp)
    {
        var spec = new GetHostPaymentsSpecification(hqp, hostId);
        var (hostPayments, totalCount) = await hostPaymentRepository.FindWithTotalCountAsync(spec);
        var hostPaymentsDto = Mapper.Map<List<HostPaymentResponse>>(hostPayments);
        return new PaginatedList<HostPaymentResponse>(hostPaymentsDto, totalCount, hqp.PageIndex, hqp.PageSize);
    }
}