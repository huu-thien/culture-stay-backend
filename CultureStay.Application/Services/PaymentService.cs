using AutoMapper;
using CultureStay.Application.Common;
using CultureStay.Application.Common.Helpers;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Common.Models;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Booking.Specifications;
using CultureStay.Application.ViewModels.Payment.Config;
using CultureStay.Application.ViewModels.Payment.QueryParameters;
using CultureStay.Application.ViewModels.Payment.Request;
using CultureStay.Application.ViewModels.Payment.Response;
using CultureStay.Application.ViewModels.Payment.Specifications;
using CultureStay.Application.ViewModels.PaymentInfo.Response;
using CultureStay.Domain.Constants;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Enum;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Message = CultureStay.Application.Common.Interfaces.Message;


namespace CultureStay.Application.Services;

public class PaymentService (
    IRepositoryBase<BookingPayment> bookingPaymentRepository,
    IRepositoryBase<Booking> bookingRepository,
    IRepositoryBase<VnpHistory> vnpHistoryRepository,
    IRepositoryBase<ChargePayment> chargePaymentRepository,
    IRepositoryBase<RefundPayment> refundPaymentRepository,
    IUnitOfWork unitOfWork,
    IOptions<PaymentConfig> paymentConfig,
    ILogger<PaymentInfoResponse> logger,
    IMailTemplateHelper mailTemplateHelper,
    IEmailSender emailSender,
    IMapper mapper,
    ICurrentUser currentUser) : BaseService(unitOfWork, mapper, currentUser), IPaymentService
{
    public async Task<PaginatedList<GetChargePaymentResponse>> ChargePayment(ChargePaymentQueryParameter pqp)
    {
        var spec = new ChargePaymentSpecification(pqp);
        var ( chargePayments,  totalCount) = await chargePaymentRepository.FindWithTotalCountAsync(spec);
        var chargePaymentsDto = Mapper.Map<List<GetChargePaymentResponse>>(chargePayments);
        return new PaginatedList<GetChargePaymentResponse>(chargePaymentsDto, totalCount, pqp.PageIndex, pqp.PageSize);
    }

    public async Task<string> CreateBookingPayment(string ip, CreateBookingPaymentRequest createBookingPaymentDto)
    {
        var booking = await bookingRepository.GetByIdAsync(createBookingPaymentDto.BookingId)
                      ?? throw new EntityNotFoundException(nameof(Booking), createBookingPaymentDto.BookingId.ToString());
       
        if (booking.Status != BookingStatus.Pending)
            throw new BadInputException("Booking is already processed");
        
        // Create DTO
        VnpHistoryDto vnpHistoryDto = new VnpHistoryDto();
        vnpHistoryDto.vnp_TxnRef = DateTime.UtcNow.Ticks;
        vnpHistoryDto.vnp_OrderInfo = "#" + vnpHistoryDto.vnp_TxnRef + " | " + "Thanh toan dat phong #" + booking.Id;
        vnpHistoryDto.vnp_Amount = (long)booking.TotalPrice;
        vnpHistoryDto.vnp_BankCode = createBookingPaymentDto.BankCode!;
        vnpHistoryDto.vnp_TmnCode = paymentConfig.Value.VnpTmnCode;
        //vnpHistoryDTO.BookingPaymentId= createBookingPaymentDto.BookingId;
        vnpHistoryDto.vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");

        //Build URL for VNPAY
        VnPayLibrary vnpay = new VnPayLibrary();
        vnpay.AddRequestData("vnp_Version", VnPayLibrary.Version);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnpHistoryDto.vnp_TmnCode);
        // Must multiply by 100 to send to vnpay system
        vnpay.AddRequestData("vnp_Amount", (vnpHistoryDto.vnp_Amount * 100).ToString());
        vnpay.AddRequestData("vnp_BankCode", vnpHistoryDto.vnp_BankCode);
        vnpay.AddRequestData("vnp_CreateDate", vnpHistoryDto.vnp_CreateDate);
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_IpAddr", ip);
        vnpay.AddRequestData("vnp_OrderInfo", vnpHistoryDto.vnp_OrderInfo);
        vnpay.AddRequestData("vnp_ReturnUrl", paymentConfig.Value.VnpReturnUrl);
        vnpay.AddRequestData("vnp_TxnRef", vnpHistoryDto.vnp_TxnRef.ToString());
        vnpay.AddRequestData("vnp_OrderType", "other");

        //Create url for VNPAY
        string paymentUrl = vnpay.CreateRequestUrl(paymentConfig.Value.VnpUrl, paymentConfig.Value.VnpHashSecret, vnpHistoryDto);

        //mapping
        VnpHistory vnpHistory = Mapper.Map<VnpHistory>(vnpHistoryDto);

        // Create bookingPayment entity
        var bookingPayment = await bookingPaymentRepository.FindOneAsync(new BookingPaymentByBookingIdSpecification(createBookingPaymentDto.BookingId));
        if (bookingPayment == null)
        {
            bookingPayment ??= new BookingPayment()
            {
                PaymentCode = Guid.NewGuid().ToString(),
                GuestId = booking.GuestId,
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                Status = BookingPaymentStatus.Pending,
                VnpHistories = new List<VnpHistory>()
            };
            bookingPaymentRepository.Add(bookingPayment);
        }
        bookingPayment.VnpHistories.Clear();
        bookingPayment.VnpHistories.Add(vnpHistory);
        
        // save payment into db
        await unitOfWork.SaveChangesAsync();

        return paymentUrl;
    }

    public async Task ReceiveDataFromVnp(VnPayReturnDto vnpayReturnDto)
    {

        // TODO: update gold & payment status
        if (PaymentConst.VnpTransactionStatusSuccess.Equals(vnpayReturnDto.vnp_ResponseCode)
            && PaymentConst.VnpTransactionStatusSuccess.Equals(vnpayReturnDto.vnp_TransactionStatus))
        {

            logger.LogInformation("giao dich thanh cong");
            VnpHistory? vnpHistory = await vnpHistoryRepository.FindOneAsync(new VnpHistoryGetByTxnRef(vnpayReturnDto.vnp_TxnRef));
            if (vnpHistory == null)
            {
                logger.LogError("Khong ton tai payment #" + vnpayReturnDto.vnp_TxnRef);
                return;
            }

            bool checkSignature = vnpHistory.vnp_SecureHash!.Equals(vnpHistory.vnp_SecureHash);
            bool isHandledOrder = PaymentConst.VnpTransactionStatusSuccess.Equals(vnpHistory.vnp_TransactionStatus);

            if (checkSignature && !isHandledOrder)
            {
                vnpHistory.vnp_TransactionStatus = vnpayReturnDto.vnp_TransactionStatus;
                vnpHistory.BookingPayment.Status = BookingPaymentStatus.Paid;
                vnpHistoryRepository.Update(vnpHistory);

                var spec = new BookingByIdSpecification(vnpHistory.BookingPayment.BookingId);
                var booking = await bookingRepository.FindOneAsync(spec);
                booking!.Status = BookingStatus.Confirmed;
                booking.Guid = Guid.NewGuid().ToString();
                await unitOfWork.SaveChangesAsync();
                
                // Send email to guest
                var guest = booking.Guest;
                var property = booking.Property;
                var host = booking.Property.Host;
                
                var bookingInfo = mailTemplateHelper.GetBookingInfoTemplate(
                    guest.User.FullName,
                    booking.CreatedAt,
                    property.Title,
                    booking.CheckInDate,
                    booking.CheckOutDate,
                    property.Address,
                    property.City,
                    host.User.FullName,
                    host.User.PhoneNumber ?? "",
                    booking.TotalPrice);
                
                var message = new Message(new List<string> {guest.User.Email!}, "Culture Stay - Thông tin đặt phòng", bookingInfo);
                await emailSender.SendEmailAsync(message);
            }
            else
            {
                logger.LogInformation("signature: " + checkSignature + ", order was handled: " + isHandledOrder);
            }
        }
        else
        {
            logger.LogError("Have some errors in transaction");
        }
    }

    public async Task<PaginatedList<GetRefundPaymentResponse>> RefundPayment(RefundPaymentQueryParameter pqp)
    {
        var spec = new RefundPaymentSpecification(pqp);
        var (refundPayments, totalCount) = await refundPaymentRepository.FindWithTotalCountAsync(spec);
        var refundPaymentsDto = Mapper.Map<List<GetRefundPaymentResponse>>(refundPayments);
        return new PaginatedList<GetRefundPaymentResponse>(refundPaymentsDto, totalCount, pqp.PageIndex, pqp.PageSize);
    }
}