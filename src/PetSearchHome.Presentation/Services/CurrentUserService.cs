namespace PetSearchHome.Presentation.Services;

// дані про поточного користувача
// впродовж однієї сесії додатку
public class CurrentUserService
{
    public int? UserId { get; private set; }
    public string? UserEmail { get; private set; }
    public bool RememberMe { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool IsGuest { get; private set; }

    public bool IsLoggedIn => UserId.HasValue;
    public bool IsAuthorized => IsLoggedIn || IsGuest;

    public void SetUser(int userId, string email)
    {
        SetUser(userId, email, rememberMe: false, isAdmin: false);
    }

    public void SetUser(int userId, string email, bool rememberMe)
    {
        SetUser(userId, email, rememberMe, isAdmin: false);
    }

    public void SetUser(int userId, string email, bool rememberMe, bool isAdmin)
    {
        UserId = userId;
        UserEmail = email;
        RememberMe = rememberMe;
        IsAdmin = isAdmin;
        IsGuest = false;
    }

    public void ClearUser()
    {
        UserId = null;
        UserEmail = null;
        RememberMe = false;
        IsAdmin = false;
        IsGuest = false;
    }

    public void SetGuest()
    {
        UserId = null;
        UserEmail = null;
        RememberMe = false;
        IsAdmin = false;
        IsGuest = true;
    }
}
