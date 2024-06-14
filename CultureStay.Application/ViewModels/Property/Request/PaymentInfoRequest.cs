namespace CultureStay.Application.ViewModels.Property.Request;

public class PaymentInfoRequest
{
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
}