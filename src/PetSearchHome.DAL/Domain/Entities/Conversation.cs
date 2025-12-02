namespace PetSearchHome.DAL.Domain.Entities;

public class Conversation
{
    public int Id { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }
    public int? ListingId { get; set; }
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public RegisteredUser User1 { get; set; } = null!;
    public RegisteredUser User2 { get; set; } = null!;
    public Listing? Listing { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

