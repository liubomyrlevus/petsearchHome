namespace PetSearchHome.BLL.DTOs;

public class LoginResultDto
{
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }

    public UserProfileDto User { get; set; } = new(); 
    public string Token { get; set; } = string.Empty;
}