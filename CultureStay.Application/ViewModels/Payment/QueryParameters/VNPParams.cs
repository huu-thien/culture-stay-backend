using CultureStay.Application.Common;

namespace CultureStay.Application.ViewModels.Payment.Response;

public class VNPParams : PagingParameters
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }
}