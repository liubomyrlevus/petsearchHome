namespace PetSearchHome.BLL.DTOs;

// Об'єкт для передачі даних про повідомлення на UI.
public class MessageDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}