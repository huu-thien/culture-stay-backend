using Microsoft.Extensions.Hosting;

namespace CultureStay.Application.Common.Helpers;


public enum TemplateType
{
    ResetPassword,
    ForgotPassword,
    BookingInfo,
    GuestCancellation,
    GuestCancellationHostNoti
}

public interface IMailTemplateHelper
{
    string GetBookingInfoTemplate(
        string guestName,
        DateTime bookingDate,
        string propertyName,
        DateTime checkInDate,
        DateTime checkOutDate,
        string address,
        string city,
        string hostName,
        string hostNumber);

    string GetGuestCancellationTemplate(
        string guestName,
        DateTime cancellationDate,
        string propertyName,
        DateTime checkInDate,
        DateTime checkOutDate,
        string address,
        string city,
        string hostName,
        string hostNumber);

    string GetGuestCancellationHostNotiTemplate(
        string guestName,
        DateTime cancellationDate,
        string propertyName,
        DateTime checkInDate,
        DateTime checkOutDate,
        string address,
        string city);
}
public class MailTemplateHelper : IMailTemplateHelper
{
    private readonly IHostEnvironment _env;
        
    public MailTemplateHelper(IHostEnvironment env)
    {
        _env = env;
    }

    public string GetBookingInfoTemplate(
        string guestName,
        DateTime bookingDate,
        string propertyName,
        DateTime checkInDate,
        DateTime checkOutDate,
        string address,
        string city,
        string hostName,
        string hostNumber)
    {
        var template = GetTemplate(TemplateType.BookingInfo)
            .Replace("{{name}}", guestName)
            .Replace("{{date}}", bookingDate.ToString("hh:mm dd/MM/yyyy"))
            .Replace("{{property_name}}", propertyName)
            .Replace("{{address}}", address)
            .Replace("{{city}}", city)
            .Replace("{{checkin_date}}", checkInDate.ToString("dd/MM/yyyy"))
            .Replace("{{checkout_date}}", checkOutDate.ToString("dd/MM/yyyy"))
            .Replace("{{host_name}}", hostName)
            .Replace("{{host_number}}", hostNumber);

        return template;
    }
    
    public string GetGuestCancellationTemplate(
        string guestName,
        DateTime cancellationDate,
        string propertyName,
        DateTime checkInDate,
        DateTime checkOutDate,
        string address,
        string city,
        string hostName,
        string hostNumber)
    {
        return GetTemplate(TemplateType.GuestCancellation)
            .Replace("{{name}}", guestName)
            .Replace("{{cancellation_date}}", cancellationDate.ToString("hh:mm dd/MM/yyyy"))
            .Replace("{{property_name}}", propertyName)
            .Replace("{{address}}", address)
            .Replace("{{city}}", city)
            .Replace("{{checkin_date}}", checkInDate.ToString("dd/MM/yyyy"))
            .Replace("{{checkout_date}}", checkOutDate.ToString("dd/MM/yyyy"))
            .Replace("{{host_name}}", hostName)
            .Replace("{{host_number}}", hostNumber);
    }
    
    public string GetGuestCancellationHostNotiTemplate(
        string guestName,
        DateTime cancellationDate,
        string propertyName,
        DateTime checkInDate,
        DateTime checkOutDate,
        string address,
        string city)
    {
        return GetTemplate(TemplateType.GuestCancellationHostNoti)
            .Replace("{{name}}", guestName)
            .Replace("{{cancellation_date}}", cancellationDate.ToString("hh:mm dd/MM/yyyy"))
            .Replace("{{property_name}}", propertyName)
            .Replace("{{address}}", address)
            .Replace("{{city}}", city)
            .Replace("{{checkin_date}}", checkInDate.ToString("dd/MM/yyyy"))
            .Replace("{{checkout_date}}", checkOutDate.ToString("dd/MM/yyyy"));
    }
    public string GetTemplate(TemplateType templateName)
    {
        var pathToFile = GetTemplatePath(templateName.ToString());
        
        using var reader = File.OpenText(pathToFile);
        return reader.ReadToEnd();
    }
    private string GetTemplatePath(string templateName)
    {
        return $"{_env.ContentRootPath}{Path.DirectorySeparatorChar}wwwroot{Path.DirectorySeparatorChar}MailTemplates{Path.DirectorySeparatorChar}{templateName}.html";
    }
}
