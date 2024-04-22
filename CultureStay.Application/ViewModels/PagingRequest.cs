using CultureStay.Domain;

namespace CultureStay.Application.ViewModels;

public class PagingRequest : IRequest
{
    public int PageNumber { get; set; } = AppConstant.DefaultPageNumber;
    
    public int PageSize { get; set; } = AppConstant.DefaultPageSize;
    
    public bool IsDescending { get; set; } = false;
}
