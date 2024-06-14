using System.Text.Json.Serialization;
using CultureStay.Application.Common;
using CultureStay.Application.ViewModels.Payment.Enums;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Payment.QueryParameters;

public class RefundPaymentQueryParameter : PagingParameters
{
    public int? UserId { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RefundPaymentStatus? Status { get; set; }
    public PaymentSortBy? OrderBy { get; set; }
}