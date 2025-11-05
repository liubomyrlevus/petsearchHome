namespace PetSearchHome.BLL.DTOs;
public class LoginResultDto
{
    public UserProfileDto User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
}