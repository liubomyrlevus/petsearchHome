namespace PetSearchHome.BLL.DTOs;

public class UserSummaryDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
}
