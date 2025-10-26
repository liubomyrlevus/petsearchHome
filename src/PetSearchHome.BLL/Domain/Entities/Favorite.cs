namespace PetSearchHome.BLL.Domain.Entities;

public class Favorite
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int ListingId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
