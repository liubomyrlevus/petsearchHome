namespace PetSearchHome.BLL.DTOs;
public class MessageDto
{
    public int Id { get; set; } 
    public int SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
// Об'єкт для передачі даних про повідомлення на UI.
