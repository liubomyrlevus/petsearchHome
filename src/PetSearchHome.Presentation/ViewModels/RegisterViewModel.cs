using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Features.Auth.Commands;
using PetSearchHome.Presentation.Services;

namespace PetSearchHome.ViewModels;

public partial class RegisterViewModel : ObservableValidator
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;
    private readonly CurrentUserService _currentUserService;

    public RegisterViewModel(
        IMediator mediator,
        NavigationManager navigationManager,
        CurrentUserService currentUserService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _selectedUserType = "Individual";
    }

    [ObservableProperty] private string _selectedUserType;

    [ObservableProperty]
    [Required(ErrorMessage = "Email обов'язковий.")]
    [EmailAddress(ErrorMessage = "Невірний формат Email.")]
    private string _email = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Пароль обов'язковий.")]
    [MinLength(6, ErrorMessage = "Пароль має містити щонайменше 6 символів.")]
    private string _password = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Підтвердьте пароль.")]
    [property: Compare(nameof(Password), ErrorMessage = "Паролі не співпадають.")]
    private string _confirmPassword = string.Empty;

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _phone = string.Empty;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private string _district = string.Empty;

    [ObservableProperty] private string _shelterName = string.Empty;
    [ObservableProperty] private string _contactPerson = string.Empty;
    [ObservableProperty] private string _shelterPhone = string.Empty;
    [ObservableProperty] private string _address = string.Empty;

    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isBusy;

    [RelayCommand]
    public async Task RegisterAsync()
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
            LoginResultDto? loginResult = null;

            if (SelectedUserType == "Individual")
            {
                var command = new RegisterIndividualCommand
                {
                    Email = Email,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    Phone = Phone,
                    City = City,
                    District = District
                };
                loginResult = await _mediator.Send(command).ConfigureAwait(false);
            }
            else if (SelectedUserType == "Shelter")
            {
                var command = new RegisterShelterCommand
                {
                    Email = Email,
                    Password = Password,
                    Name = ShelterName,
                    ContactPerson = ContactPerson,
                    Phone = ShelterPhone,
                    Address = Address
                };
                loginResult = await _mediator.Send(command).ConfigureAwait(false);
            }

            if (loginResult != null && loginResult.IsSuccess)
            {
                _currentUserService.Login(new UserDto
                {
                    Id = loginResult.User.Id,
                    Email = loginResult.User.Email,
                    IsAdmin = loginResult.User.IsAdmin,
                    Name = SelectedUserType == "Shelter" ? ShelterName : $"{FirstName} {LastName}"
                });

                _navigationManager.NavigateTo("/home", forceLoad: true, replace: true);
            }
            else
            {
                ErrorMessage = loginResult?.Error ?? "Сталася помилка під час реєстрації.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
