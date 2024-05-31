using System.Text.Json.Serialization;
using CultureStay.Application.Common;
using CultureStay.Application.ViewModels.Cancellation.Enums;
using CultureStay.Domain.Enum;

namespace CultureStay.Application.ViewModels.Cancellation.QueryParameters;

public class CancellationTicketQueryParameters : PagingParameters
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CancellationTicketSortBy? OrderBy { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CancellationTicketStatus? Status { get; set; }
    
    public bool? IsGuest { get; set; }
    
    public int? HostId { get; set; }
    
    public int? IssuerId { get; set; }
}