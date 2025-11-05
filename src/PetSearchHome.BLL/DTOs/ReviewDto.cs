namespace PetSearchHome.BLL.DTOs;
public class ReviewDto
{
    public int Id { get; set; } 
    public int ReviewerId { get; set; } 
    public string ReviewerName { get; set; } = string.Empty;
    public string? ReviewerAvatarUrl { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
// Об'єкт для передачі даних про відгук на UI.
