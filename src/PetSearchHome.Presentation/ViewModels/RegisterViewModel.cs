using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.Commands;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PetSearchHome.ViewModels;

public partial class RegisterViewModel : ObservableValidator
{
    private readonly IMediator _mediator;

    public RegisterViewModel(IMediator mediator)
    {
        _mediator = mediator;
        _selectedUserType = "Individual";
    }

    [ObservableProperty]
    private string _selectedUserType;

    [ObservableProperty]
    [Required(ErrorMessage = "Поле Email є обов'язковим.")]
    [EmailAddress(ErrorMessage = "Введіть коректний Email.")]
    private string _email = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Вкажіть пароль.")]
    [MinLength(6, ErrorMessage = "Пароль має містити щонайменше 6 символів.")]
    private string _password = string.Empty;

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
    private async Task RegisterAsync()
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

                await _mediator.Send(command);
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

                await _mediator.Send(command);
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
