using MediatR;
using PetSearchHome.BLL.DTOs;
namespace PetSearchHome.BLL.Commands;
public class SendMessageCommand : IRequest<MessageDto>
{
    public int ConversationId { get; set; } // ОНОВЛЕНО
    public int SenderId { get; set; } // ОНОВЛЕНО
    public string Content { get; set; } = string.Empty;
}

//надсилання повідомлення
