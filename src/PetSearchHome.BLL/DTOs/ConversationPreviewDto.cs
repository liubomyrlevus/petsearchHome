namespace PetSearchHome.BLL.DTOs;

public class ConversationPreviewDto
{
    public int ConversationId { get; set; }
    public int OtherParticipantId { get; set; }
    public string OtherParticipantName { get; set; } = string.Empty;
    public string? OtherParticipantAvatarUrl { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
}

