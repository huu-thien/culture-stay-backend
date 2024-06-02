using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Application.Services.Interface;
using CultureStay.Application.ViewModels.Chat.Response;
using CultureStay.Application.ViewModels.Chat.Specifications;
using CultureStay.Domain.Entities;
using CultureStay.Domain.Exceptions;
using CultureStay.Domain.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Message = CultureStay.Domain.Entities.Message;

namespace CultureStay.Application.Services;

public class ChatService(
    IRepositoryBase<Message> messageRepository,
    ICurrentUser currentUser,
    IMapper mapper, 
    IUnitOfWork unitOfWork,
    UserManager<User> userManager
    ) : BaseService(unitOfWork, mapper, currentUser), IChatService
{
    public async Task<List<GetMessageResponse>> GetMessagesWithUserIdAsync(int userId)
    {
        var currentUserId = int.Parse(CurrentUser.Id!);
        
        var isUserExisted = await userManager.Users.AnyAsync(u => u.Id == userId);
        if (!isUserExisted)
            throw new EntityNotFoundException(nameof(User), userId.ToString());
        
        var messages = await messageRepository
            .FindListAsync(new MessageWithUserIdSpecification(currentUserId, userId));
        
        return Mapper.Map<List<GetMessageResponse>>(messages);
    }

    public async Task<List<GetContactResponse>> GetContactsAsync()
    {
        var currentUserId = int.Parse(CurrentUser.Id!);
        
        var contactUsers = await userManager.Users
            .Where(u => u.SentMessages.Any(m => m.ReceiverId == currentUserId) ||
                        u.ReceivedMessages.Any(m => m.SenderId == currentUserId))
            .ToListAsync();
        
        var contactResponses = Mapper.Map<List<GetContactResponse>>(contactUsers);

        foreach (var contact in contactResponses)
        {
            var lastMessage = await messageRepository
                .FindOneAsync(new MessageWithUserIdSpecification(currentUserId, contact.Id, true));
            contact.LastMessage = lastMessage?.Content ?? string.Empty;
            contact.LastMessageTime = lastMessage?.CreatedAt ?? DateTime.MinValue;
        }
        return contactResponses.OrderByDescending(_ => _.LastMessageTime).ToList();
    }
}