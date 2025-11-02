namespace PetSearchHome.BLL.DTOs;

// Об'єкт для прев'ю розмови у списку чатів.
public class ConversationPreviewDto
{
    public Guid ConversationId { get; set; }
    public string OtherParticipantName { get; set; }
    public string? OtherParticipantAvatarUrl { get; set; }
    public string LastMessage { get; set; }
    public DateTime LastMessageAt { get; set; }
}