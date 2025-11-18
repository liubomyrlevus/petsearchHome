using MediatR;
using PetSearchHome.BLL.DTOs;

namespace PetSearchHome.BLL.Commands;

public class SendMessageCommand : IRequest<MessageDto>
{
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
}

