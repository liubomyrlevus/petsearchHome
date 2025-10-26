namespace PetSearchHome.BLL.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public Guid ReviewerId { get; set; }

    public Guid ReviewedId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsModerated { get; set; }
}
