using CultureStay.Application.Common;
using CultureStay.Application.Common.Models;
using CultureStay.Application.ViewModels.Host.Response;

namespace CultureStay.Application.Services.Interface;

public interface IHostService
{
    Task<GetHostResponse> GetHostByIdAsync(int id);
    Task<GetHostResponse> GetHostByUserIdAsync(int userId);
    Task<PaginatedList<GetHostForAdminResponse>> GetHostsForAdminAsync(PagingParameters pp);
    Task<bool> CheckHostIsStayedAsync(int hostId);
}