using AutoMapper;
using CultureStay.Application.Common.Helpers;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Common.Specifications;
using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Application.ViewModels.Cancellation.QueryParameters;
using CultureStay.Application.ViewModels.Cancellation.Request;
using CultureStay.Application.ViewModels.Cancellation.Response;
using CultureStay.Application.ViewModels.Cancellation.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Exceptions.BookingException;
using CultureStay.Domain.Repositories.Base;
using Message = CultureStay.Application.Common.Interfaces.Message;

namespace CultureStay.Application.Services.Interface;

public class CancellationService (
    IRepositoryBase<CancellationTicket> cancellationTicketRepository,
    IRepositoryBase<Booking> bookingRepository,
    IRepositoryBase<Guest> guestRepository,
    IRepositoryBase<Host> hostRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUser currentUser,
    IEmailSender emailSender,
    IMailTemplateHelper mailTemplateHelper
        ) : BaseService(unitOfWork, mapper, currentUser), ICancellationService
{
    public async Task<GetCancellationResponse> CreateCancellationTicketAsync(CreateCancellationRequest request)
    {
        var booking = await ValidateCreateCancellation(request);
        var cancellationTicket = Mapper.Map<CancellationTicket>(request);

        cancellationTicket.Status = CancellationTicketStatus.Pending;
        cancellationTicket.IsIssuerGuest = request.IsGuest;
        cancellationTicket.TheOtherPartyId = request.IsGuest ? booking.Property.Host.UserId : booking.Guest.UserId;
        
        booking.Status = booking.Status == BookingStatus.CheckedIn 
            ? BookingStatus.CancelledAfterCheckIn 
            : BookingStatus.CancelledBeforeCheckIn;

        await unitOfWork.BeginTransactionAsync();
        try
        {
            cancellationTicketRepository.Add(cancellationTicket);
            await unitOfWork.SaveChangesAsync();
            await AcceptCancellationTicketAsync(cancellationTicket.Id, cancellationTicket.ResolveNote);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await unitOfWork.RollbackAsync();
            throw;
        }
        // Get lai cancellation ticket voi day du thong tin
        // Todo: Move mail sang background job
        cancellationTicket =
            await cancellationTicketRepository.FindOneAsync(new CancellationByIdSpecification(cancellationTicket.Id))
             ?? throw new EntityNotFoundException(nameof(CancellationTicket), cancellationTicket.Id.ToString());
        
        if (cancellationTicket.IsIssuerGuest)
        {
            await SendCancellationEmailToGuestAsync(cancellationTicket);
            await SendCancellationNotiToHostAsync(cancellationTicket);
        }

        return Mapper.Map<GetCancellationResponse>(cancellationTicket);
    }

    public async Task<PaginatedList<GetCancellationResponse>> GetCancellationTicketAsync(CancellationTicketQueryParameters ctp)
    {
        var (cancellationTickets, totalCount) =
            await cancellationTicketRepository.FindWithTotalCountAsync(new GetCancellationsSpecification(ctp));
        var cancellationTicketsResponse = Mapper.Map<List<GetCancellationResponse>>(cancellationTickets);
        return new PaginatedList<GetCancellationResponse>(cancellationTicketsResponse, totalCount, ctp.PageIndex, ctp.PageSize);
    }

    public async Task<GetCancellationResponse> GetCancellationTicketByIdAsync(int id)
    {
        var cancellationTicket = await cancellationTicketRepository.FindOneAsync(new CancellationByIdSpecification(id))
            ?? throw new EntityNotFoundException(nameof(CancellationTicket), id.ToString());
        return Mapper.Map<GetCancellationResponse>(cancellationTicket);
    }

    public async Task RejectCancellationTicketAsync(int cancellationTicketId, RejectCancellationTicketRequest request)
    {
        await UpdateCancellationTicketAsync(cancellationTicketId, request.ResolveNote, CancellationTicketStatus.Rejected);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AcceptCancellationTicketAsync(int cancellationTicketId, string? resolveNote)
    {
        await UpdateCancellationTicketAsync(cancellationTicketId, resolveNote, CancellationTicketStatus.Resolved);
    }
    private async Task<CancellationTicket> UpdateCancellationTicketAsync(int cancellationId, string? resolveNote, CancellationTicketStatus status)
    {
        var cancellationTicket = await cancellationTicketRepository
                                     .FindOneAsync(new CancellationByIdSpecification(cancellationId))
                                 ?? throw new EntityNotFoundException(nameof(CancellationTicket), cancellationId.ToString());
        
        if (cancellationTicket.Status is not CancellationTicketStatus.Pending)
            throw new BookingCancellationException("You can only reject pending cancellation tickets.");
        
        cancellationTicket.Status = status;
        cancellationTicket.ResolveNote = resolveNote;

        return cancellationTicket;
    }
    private async Task<Booking> ValidateCreateCancellation(CreateCancellationRequest request)
    {
        var booking = await bookingRepository.FindOneAsync(new BookingByIdSpecification(request.BookingId))
            ?? throw new EntityNotFoundException(nameof(Booking), request.BookingId.ToString());
        
        if(booking.Status is BookingStatus.Completed or BookingStatus.CancelledBeforeCheckIn or BookingStatus.CancelledAfterCheckIn or BookingStatus.Rejected)
            throw new BookingCancellationException("You can only cancel bookings that have not ended yet");
        
        var currentUserId = int.Parse(currentUser.Id!);
        if (request.IsGuest)
        {
            var guest = await guestRepository.FindOneAsync(new GuestByUserIdSpecification(currentUserId))
                ?? throw new EntityNotFoundException(nameof(Guest), currentUserId.ToString());
            if(booking.GuestId != guest.Id)
                throw new BookingCancellationException("You can only cancel your own bookings");
        }
        else
        {
            var host = await hostRepository.FindOneAsync(new HostByUserIdSpecification(currentUserId)) ??
                throw new EntityNotFoundException(nameof(Host), currentUserId.ToString());
            if(booking.GuestId != host.Id)
                throw new BookingCancellationException("You can only cancel bookings that you are the host of");
        }
        if((booking.CheckOutDate.Date - DateTime.Now.Date).Days == 0)
            throw new BookingCancellationException("You can only cancel bookings that have not ended yet");
        return booking;
    }

    private async Task SendCancellationEmailToGuestAsync(CancellationTicket cancellationTicket)
    {
        var guest = cancellationTicket.IsIssuerGuest 
            ? await guestRepository.FindOneAsync(new GuestByUserIdSpecification(cancellationTicket.CreatedBy!.Value))
            : await guestRepository.FindOneAsync(new GuestByUserIdSpecification(cancellationTicket.TheOtherPartyId));
        
        var booking = cancellationTicket.Booking;
        var property = cancellationTicket.Booking.Property;
        var host = property.Host;

        var mailTemplate = mailTemplateHelper.GetGuestCancellationTemplate(
            guest!.User.FullName,
            booking.CreatedAt,
            booking.Property.Title,
            booking.CheckInDate,
            booking.CheckOutDate,
            property.Address,
            property.City,
            host.User.FullName,
            host.User.PhoneNumber ?? ""
            );
        
        var message = new Message(new List<string> { guest.User.Email! }, "Culture Stay - Thông tin hủy phòng",
            mailTemplate);

        await emailSender.SendEmailAsync(message);
    }
    private async Task SendCancellationNotiToHostAsync(CancellationTicket cancellationTicket)
    {
        var host = cancellationTicket.IsIssuerGuest
            ? await hostRepository.FindOneAsync(new HostByUserIdSpecification(cancellationTicket.TheOtherPartyId))
            : await hostRepository.FindOneAsync(new HostByUserIdSpecification(cancellationTicket.CreatedBy!.Value));

        var booking = cancellationTicket.Booking;
        var property = cancellationTicket.Booking.Property;
        var guest = booking.Guest;

        var template = mailTemplateHelper.GetGuestCancellationHostNotiTemplate(
            guest!.User.FullName,
            cancellationTicket.CreatedAt,
            property.Title,
            booking.CheckInDate,
            booking.CheckOutDate,
            property.Address,
            property.City);
        var message = new Message(new List<string> {host.User.Email!}, "Culture Stay - Thông tin hủy phòng", template);
        await emailSender.SendEmailAsync(message);
    }
}