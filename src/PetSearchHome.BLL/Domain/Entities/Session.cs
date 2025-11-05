namespace PetSearchHome.BLL.Domain.Entities;
public class Session
{
    public Guid Id { get; set; } 
    public int UserId { get; set; } 
    public string SessionToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public bool IsValid { get; set; } = true;

    public RegisteredUser User { get; set; } = null!;
}