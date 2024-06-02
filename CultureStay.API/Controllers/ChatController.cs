using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels;
using CultureStay.Application.ViewModels.Chat.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CultureStay.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }
    
    [HttpGet("contacts")]
    public async Task<IActionResult> GetContactsAsync()
    {
        var contacts = await _chatService.GetContactsAsync();
        return Ok(new BaseResponse<List<GetContactResponse>>{Message = "Get contacts successfully", Data = contacts});
    }
    
    [HttpGet("messages/{userId}")]
    public async Task<IActionResult> GetMessagesWithUserIdAsync(int userId)
    {
        var messages = await _chatService.GetMessagesWithUserIdAsync(userId);
        return Ok(new BaseResponse<List<GetMessageResponse>>{Message = "Get messages successfully", Data = messages});
    }
}