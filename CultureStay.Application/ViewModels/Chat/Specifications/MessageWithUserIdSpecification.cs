using CultureStay.Domain.Entities;
using CultureStay.Domain.Specification;

namespace CultureStay.Application.ViewModels.Chat.Specifications;

public class MessageWithUserIdSpecification : Specification<Message>
{
    public MessageWithUserIdSpecification(int userId1, int userId2, bool isDescending = false)
    {
        AddFilter(m => m.SenderId == userId1 && m.ReceiverId == userId2 
                       || m.SenderId == userId2 && m.ReceiverId == userId1);

        AddOrderByField(nameof(Message.CreatedAt));
        if (isDescending)
            ApplyDescending();
    }
}