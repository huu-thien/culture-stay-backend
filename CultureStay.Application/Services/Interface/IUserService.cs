using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.User.Request;
using CultureStay.Application.ViewModels.User.Response;

namespace CultureStay.Application.Services.Interface;

public interface IUserService
{
    Task<GetUsersForAdminResponse> GetUserByIdAsync(int id);
    Task<PaginatedList<GetUsersForAdminResponse>> GetUsersForAdminAsync(UserPagingParameters pp);
    Task<GetUsersForAdminResponse> UpdateUserAsync(int id, UpdateUserInfoRequest dto);
}