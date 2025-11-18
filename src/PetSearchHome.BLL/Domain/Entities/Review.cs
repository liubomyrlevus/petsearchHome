namespace PetSearchHome.BLL.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int ReviewerId { get; set; }
    public int ReviewedId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsModerated { get; set; }

    public RegisteredUser Reviewer { get; set; } = null!;
    public RegisteredUser Reviewed { get; set; } = null!;
}

