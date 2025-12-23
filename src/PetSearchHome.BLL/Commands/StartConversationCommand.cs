using MediatR;

namespace PetSearchHome.BLL.Commands;

public class StartConversationCommand : IRequest<int>
{
    public int InitiatorUserId { get; set; }
    public int ReceiverUserId { get; set; }
    public int ListingId { get; set; }
}

