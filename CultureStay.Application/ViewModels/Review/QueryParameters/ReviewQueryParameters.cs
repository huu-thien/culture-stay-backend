using System.Text.Json.Serialization;
using CultureStay.Application.Common;
using CultureStay.Application.ViewModels.Review.Enums;

namespace CultureStay.Application.ViewModels.Review.QueryParameters;

public class ReviewQueryParameters : PagingParameters
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReviewSortBy? OrderBy { get; set; }
}