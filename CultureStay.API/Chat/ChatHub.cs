using System.Security.Claims;
using System.Text.RegularExpressions;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Domain.Repositories.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Message = CultureStay.Domain.Entities.Message;

namespace CultureStay.Chat;


[Authorize]
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("UserConnected", $"Context.ConnectionId");
    }
    
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    } 
    

    
    public async Task SendMessageToUser(string user, string message, IRepositoryBase<Message> chatRepository, IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        var senderId = Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("senderId", senderId);
        var messageEntity = new Message()
        {
            SenderId = int.Parse(senderId!),
            SenderAvatarUrl = Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Uri)?.Value,
            SenderName = Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!,
            ReceiverId = int.Parse(user),
            Content = message
        };
        
        chatRepository.Add(messageEntity);
        await unitOfWork.SaveChangesAsync();
        
        var messageViewModel = new MessageViewModel
        {
            Content = Regex.Replace(message, @"<.*?>", string.Empty),
            FromUserId = int.Parse(senderId!),
            FromUserName = Context.User?.Identity?.Name ?? string.Empty,
            FromUserAvatar = Context.User!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Uri)?.Value,
            ToUserId = int.Parse(user)
        };
        await Clients.User(user).SendAsync("ReceiveMessage", messageViewModel);
        await Clients.Caller.SendAsync("ReceiveMessage", messageViewModel);
    }
}