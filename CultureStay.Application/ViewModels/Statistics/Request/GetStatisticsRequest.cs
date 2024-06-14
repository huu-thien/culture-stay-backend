namespace CultureStay.Application.ViewModels.Statistics.Request;

public class GetStatisticsRequest
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}