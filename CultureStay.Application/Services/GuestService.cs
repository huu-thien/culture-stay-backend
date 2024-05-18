using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Application.ViewModels.Guest.Response;
using CultureStay.Application.ViewModels.Guest.Specifications;
using CultureStay.Application.ViewModels.Review.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public class GuestService (
    IRepositoryBase<Guest> guestRepository,
    IRepositoryBase<Host> hostRepository,
    IRepositoryBase<Booking> bookingRepository,
    IRepositoryBase<GuestReview> guestReviewRepository,
    ICurrentUser currentUser,
    IMapper mapper, IUnitOfWork unitOfWork
    ) : BaseService(unitOfWork, mapper, currentUser), IGuestService
{
    public async Task<GetGuestResponse> GetGuestByIdAsync(int id)
    {
        var guest = await guestRepository.FindOneAsync(new GuestDetailSpecification(id));
        if(guest is null)
            throw new EntityNotFoundException(nameof(Guest), id.ToString());
        
        var result = Mapper.Map<GetGuestResponse>(guest);
        result.NumberOfReviews = await guestReviewRepository.CountAsync(gr => gr.GuestId == id);
        if(result.NumberOfReviews > 0)
            result.Rating = await guestReviewRepository
                .AverageAsync(new GuestReviewSpecification(id), gr => gr.Rating);
        return result;
    }

    public async Task<bool> CheckGuestIsStayedAsync(int guestId)
    {
        var currentUserId = int.TryParse(currentUser.Id, out var id) ? id : 0;
        if(currentUserId == 0) return false;

        var host = await hostRepository.FindOneAsync(new HostByUserIdSpecification(currentUserId));
        if (host is null) return false;
        
        var spec = new IsGuestStayedInHostPropertySpecification(host.Id, guestId);
        return await bookingRepository.AnyAsync(spec);
    }
}