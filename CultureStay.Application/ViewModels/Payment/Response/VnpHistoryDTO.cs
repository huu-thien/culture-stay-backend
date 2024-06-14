namespace CultureStay.Application.ViewModels.Payment.Response;

public class VnpHistoryDto
{
    public long vnp_TxnRef { get; set; }

    public string vnp_OrderInfo { get; set; } = null!;

    public long vnp_Amount { get; set; }

    public string vnp_BankCode { get; set; } = null!;
    public string vnp_BankTranNo { get; set; } = null!;

    public string vnp_CardType { get; set; } = null!;

    public long vnp_PayDate { get; set; }

    public string vnp_ResponseCode { get; set; } = null!;

    public string vnp_TmnCode { get; set; } = null!;

    public string vnp_TransactionNo { get; set; } = null!;

    public string vnp_TransactionStatus { get; set; } = null!;

    public string vnp_SecureHash { get; set; } = null!;
    public string vnp_CreateDate { get; set; } = null!;
    public int BookingPaymentId { get; set; }
}