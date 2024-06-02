using CultureStay.Application.ViewModels.Chat.Response;

namespace CultureStay.Application.Services.Interface;

public interface IChatService
{
    Task<List<GetMessageResponse>> GetMessagesWithUserIdAsync(int userId);
    Task<List<GetContactResponse>> GetContactsAsync();
}