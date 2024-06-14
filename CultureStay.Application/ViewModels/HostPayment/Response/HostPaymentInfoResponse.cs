namespace CultureStay.Application.ViewModels.HostPayment.Response;

public class HostPaymentInfoResponse
{
    public int HostId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
}