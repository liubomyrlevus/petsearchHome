using MediatR;

namespace PetSearchHome.BLL.Commands;

public class StartConversationCommand : IRequest<Guid>
{
    public Guid InitiatorUserId { get; set; }
    public Guid ReceiverUserId { get; set; }
    public int ListingId { get; set; }
}