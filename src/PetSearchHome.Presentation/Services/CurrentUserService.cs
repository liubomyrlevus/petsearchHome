namespace PetSearchHome.Presentation.Services;

// дані про поточного користувача
// впродовж однієї сесії додатку
public class CurrentUserService
{
    public int? UserId { get; private set; }
    public string? UserEmail { get; private set; }

    public bool IsLoggedIn => UserId.HasValue;

    public void SetUser(int userId, string email)
    {
        UserId = userId;
        UserEmail = email;
    }

    public void ClearUser()
    {
        UserId = null;
        UserEmail = null;
    }
}