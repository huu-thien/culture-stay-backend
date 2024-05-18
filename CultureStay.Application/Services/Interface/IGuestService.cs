using CultureStay.Application.ViewModels.Guest.Response;

namespace CultureStay.Application.Services.Interface;

public interface IGuestService
{
    Task<GetGuestResponse> GetGuestByIdAsync(int id);
    Task<bool> CheckGuestIsStayedAsync(int guestId);
}