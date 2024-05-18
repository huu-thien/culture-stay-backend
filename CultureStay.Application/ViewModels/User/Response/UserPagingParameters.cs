using CultureStay.Application.Common;

namespace CultureStay.Application.ViewModels.User.Response;

public class UserPagingParameters : PagingParameters
{
    public  bool IsHostOnly { get; set; }
}