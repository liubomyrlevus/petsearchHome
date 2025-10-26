namespace PetSearchHome.BLL.Features.Auth.DTOs;

public sealed class LoginResultDto
{
    public bool IsSuccess { get; init; }

    public string? Error { get; init; }

    public string? Token { get; init; }
}
