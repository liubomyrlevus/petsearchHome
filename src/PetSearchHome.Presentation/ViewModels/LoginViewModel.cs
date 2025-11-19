using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace PetSearchHome.ViewModels // ❗ Namespace PetSearchHome.ViewModels
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
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
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
            var loginResult = await _mediator.Send(query).ConfigureAwait(false);
            if (loginResult.IsSuccess) // Припускаємо, що ви оновили LoginResultDto
            {
                _currentUserService.Login(new UserDto
                {
                    Id = loginResult.User.Id,
                    Email = loginResult.User.Email,
                    Name = loginResult.User.Email, // Або ім'я з профілю
                    IsAdmin = loginResult.User.IsAdmin
                });

                _navigationManager.NavigateTo("/home", replace: true);
            }
            else
            {
                ErrorMessage = loginResult.Error ?? "Невірний логін або пароль.";
            }
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
