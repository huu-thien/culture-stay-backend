using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.PaymentInfo.Response;
using CultureStay.Application.ViewModels.PaymentInfo.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public class PaymentInfoService( IRepositoryBase<PaymentInfo> paymentInfoRepository,
    IRepositoryBase<Host> hostRepository,
    ICurrentUser currentUser,
    IMapper mapper, 
    IUnitOfWork unitOfWork) : BaseService(unitOfWork, mapper, currentUser), IPaymentInfoService
{
    public async Task<PaymentInfoResponse> GetPaymentInfoAsync()
    {
        var host = await ValidateHost();
        if(host.PaymentInfo is null)
            throw new EntityNotFoundException(nameof(PaymentInfo), host.UserId.ToString());
        return Mapper.Map<PaymentInfoResponse>(host.PaymentInfo);
    }

    public async Task<PaymentInfoResponse> CreatePaymentInfoAsync(PaymentInfoResponse paymentInfoDto)
    {
        var host = await ValidateHost();
        if (host.PaymentInfo is null)
        {
            PaymentInfo paymentInfo = Mapper.Map<PaymentInfo>(paymentInfoDto);
            paymentInfo.HostId = host.Id;
            paymentInfoRepository.Add(paymentInfo);
            await unitOfWork.SaveChangesAsync();
            return Mapper.Map<PaymentInfoResponse>(paymentInfo);
        }
        throw new EntityAlreadyExistedException(nameof(PaymentInfo), host.UserId.ToString());
    }

    public async Task<PaymentInfoResponse> UpdatePaymentInfoAsync(PaymentInfoResponse paymentInfoDto)
    {
        var host = await ValidateHost();
        var spec = new PaymentInfoSpecification(host.Id);
        var paymentInfo = await paymentInfoRepository.FindOneAsync(spec);
        if (paymentInfo is null)
            throw new EntityNotFoundException(nameof(PaymentInfo), host.UserId.ToString());
        Mapper.Map(paymentInfoDto, paymentInfo);
        await unitOfWork.SaveChangesAsync();
        return Mapper.Map<PaymentInfoResponse>(host.PaymentInfo);
    }
    
    public async Task<Host> ValidateHost()
    {
        int userId = int.Parse(currentUser.Id!);
        var spec = new HostByUserIdSpecification(userId);
        var host = await hostRepository.FindOneAsync(spec);
        if (host is null)
            throw new EntityNotFoundException(nameof(Host), userId.ToString());
        return host;
    }
}