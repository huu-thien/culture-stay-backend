using AutoMapper;
using CultureStay.Application.Common;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Application.ViewModels.Host.Response;
using CultureStay.Application.ViewModels.Host.Specifications;
using CultureStay.Application.ViewModels.Review.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public class HostService(IRepositoryBase<Host> hostRepository, 
    IRepositoryBase<HostReview> hostReviewRepository, 
    IRepositoryBase<Guest> guestRepository, 
    IRepositoryBase<Property> propertyRepository, 
    IRepositoryBase<Booking> bookingRepository,
    IUnitOfWork unitOfWork, IMapper mapper, ICurrentUser currentUser
    ) : BaseService(unitOfWork, mapper, currentUser), IHostService
{
    public async Task<GetHostResponse> GetHostByIdAsync(int id)
    {
        var host = await hostRepository.FindOneAsync(new HostDetailSpecification(id));
        if (host is null)
            throw new EntityNotFoundException(nameof(Host), id.ToString());
        var result = Mapper.Map<GetHostResponse>(host);
        result.NumberOfReviews = await hostReviewRepository.CountAsync(h => h.HostId == id);
        if (result.NumberOfReviews > 0)
            result.Rating = await hostReviewRepository
                .AverageAsync(new HostReviewSpecification(id), h => h.Rating);
        return result;
    }

    public async Task<GetHostResponse> GetHostByUserIdAsync(int userId)
    {
        var spec = new HostByUserIdSpecification(userId);
        var host = await hostRepository.FindOneAsync(spec);
        
        if (host is null)
            throw new EntityNotFoundException(nameof(Host), userId.ToString());
        
        var result = Mapper.Map<GetHostResponse>(host);
        result.NumberOfReviews = await hostReviewRepository.CountAsync(h => h.HostId == host.Id);
        if (result.NumberOfReviews > 0)
            result.Rating = await hostReviewRepository
                .AverageAsync(new HostReviewSpecification(host.Id), h => h.Rating);
        return result;
    }

    public async Task<PaginatedList<GetHostForAdminResponse>> GetHostsForAdminAsync(PagingParameters pp)
    {
        var spec = new HostsPagingSpecification(pp);
        var (hosts, totalCount) = await hostRepository.FindWithTotalCountAsync(spec);
        if (hosts is null)
            throw new EntityNotFoundException(nameof(Host), pp.PageIndex.ToString());

        var result = Mapper.Map<List<GetHostForAdminResponse>>(hosts);
        foreach (var host in result)
        {
            host.NumberOfReviews = await hostReviewRepository.CountAsync(h => h.HostId == host.Id);
            if (host.NumberOfReviews > 0)
                host.Rating = await hostReviewRepository
                    .AverageAsync(new HostReviewSpecification(host.Id), h => h.Rating);
            host.NumberOfProperties = await propertyRepository.CountAsync(p => p.HostId == host.Id);
            host.NumberOfBookings = await bookingRepository.CountAsync(b => b.Property.HostId == host.Id);
        }
        return new PaginatedList<GetHostForAdminResponse>(result, totalCount, pp.PageIndex, pp.PageSize);
    }

    public async Task<bool> CheckHostIsStayedAsync(int hostId)
    {
        var currentUserId = int.TryParse(currentUser.Id, out var id) ? id : 0;
        if (currentUserId == 0) return false;
        
        var guest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(currentUserId));
        if(guest is null) return false;
        
        var spec = new IsGuestStayedInHostPropertySpecification(hostId, guest.Id);
        return  await bookingRepository.AnyAsync(spec);
    }
}