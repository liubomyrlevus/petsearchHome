using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Commands;

//надсилання повідомлення
public class SendMessageCommand : IRequest<MessageDto>
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
}