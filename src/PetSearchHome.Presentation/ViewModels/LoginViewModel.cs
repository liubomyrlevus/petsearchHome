using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PetSearchHome.ViewModels;

public partial class LoginViewModel : ObservableValidator
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;
    private readonly CurrentUserService _currentUserService;

    [ObservableProperty]
    [Required(ErrorMessage = "Поле Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Введіть коректну адресу електронної пошти.")]
    private string _email = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Поле «Пароль» є обов'язковим.")]
    [MinLength(6, ErrorMessage = "Пароль має містити щонайменше 6 символів.")]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _rememberMe;

    public bool CanLogin =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password);

    public LoginViewModel(
        IMediator mediator,
        NavigationManager navigationManager,
        CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
        _currentUserService = currentUserService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            ErrorMessage = string.Empty;
            return;
        }

        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var query = new LoginQuery { Email = Email, Password = Password };
            var loginResult = await _mediator.Send(query);

            if (!loginResult.IsSuccess)
            {
                ErrorMessage = string.IsNullOrWhiteSpace(loginResult.Error)
                    ? (string.IsNullOrWhiteSpace(loginResult.ErrorMessage)
                        ? "Неправильний email або пароль."
                        : loginResult.ErrorMessage)
                    : loginResult.Error;

                Password = string.Empty;
                return;
            }

            if (loginResult.User is null)
            {
                ErrorMessage = "Не вдалося отримати дані користувача.";
                Password = string.Empty;
                return;
            }

            _currentUserService.SetUser(
                loginResult.User.Id,
                loginResult.User.Email,
                RememberMe,
                loginResult.User.IsAdmin);

            // Повне перезавантаження Blazor WebView (аналог Ctrl+R),
            // щоб меню й сторінки одразу побачили оновлений стан користувача.
            _navigationManager.NavigateTo("/home", forceLoad: true);
        }
        catch (System.Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
