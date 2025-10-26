namespace PetSearchHome.BLL.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }

    public Guid User1Id { get; set; }

    public Guid User2Id { get; set; }

    public int? ListingId { get; set; }

    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public List<Message> Messages { get; set; } = [];
}
