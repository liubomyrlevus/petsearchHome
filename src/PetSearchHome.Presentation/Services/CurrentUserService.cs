using System;

namespace PetSearchHome.Presentation.Services;

public class UserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Id { get; set; }
    public bool IsAdmin { get; set; }
}

// дані про поточного користувача
// впродовж однієї сесії додатку
public class CurrentUserService
{

    public event Action? OnChange;
    public int? UserId { get; private set; }
    public string? UserEmail { get; private set; }
    public bool RememberMe { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool IsGuest { get; private set; }

    public string UserName => UserEmail ?? "Гість";

    public bool IsLoggedIn => UserId.HasValue;
    public bool IsAuthorized => IsLoggedIn || IsGuest;

    public void Login(UserDto user)
    {
        SetUser(user.Id, user.Email, false, user.IsAdmin);
    }

    public void Logout()
    {
        ClearUser();
    }

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

        NotifyStateChanged();
    }

    public void ClearUser()
    {
        UserId = null;
        UserEmail = null;
        RememberMe = false;
        IsAdmin = false;
        IsGuest = false;
        NotifyStateChanged();
    }

    public void SetGuest()
    {
        UserId = null;
        UserEmail = null;
        RememberMe = false;
        IsAdmin = false;
        IsGuest = true;
        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnChange?.Invoke();
}
