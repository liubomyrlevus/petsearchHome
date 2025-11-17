namespace PetSearchHome.BLL.Domain.Entities;
public class Message
{
    public int Id { get; set; } 
    public int ConversationId { get; set; } 
    public int SenderId { get; set; } 
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Conversation Conversation { get; set; } = null!;
    public RegisteredUser Sender { get; set; } = null!;
}