using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Repositories.Base;
using Quartz;

namespace CultureStay.Application.BackgroundServices;

public class RemovePendingBookingJob : IJob
{
    private readonly IRepositoryBase<Booking> _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemovePendingBookingJob(IRepositoryBase<Booking> bookingRepository, IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        var bookings = await _bookingRepository.FindListAsync(new ExpiredBookingSpecification());
        _bookingRepository.DeleteRange(bookings);
        await _unitOfWork.SaveChangesAsync();
    }
}