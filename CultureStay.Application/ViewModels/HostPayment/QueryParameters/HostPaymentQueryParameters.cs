using System.Text.Json.Serialization;
using CultureStay.Application.Common;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.HostPayment.QueryParameters;

public class HostPaymentQueryParameters : PagingParameters
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HostPaymentStatus? Status { get; set; }
}