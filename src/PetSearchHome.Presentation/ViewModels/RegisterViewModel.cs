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
    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress]
    private string _email;

    [ObservableProperty]
    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [MinLength(6, ErrorMessage = "Пароль має бути мін. 6 символів")]
    private string _password;

    [ObservableProperty] private string _firstName;
    [ObservableProperty] private string _lastName;
    [ObservableProperty] private string _phone;
    [ObservableProperty] private string _city;
    [ObservableProperty] private string _district;

    [ObservableProperty] private string _shelterName;
    [ObservableProperty] private string _contactPerson;
    [ObservableProperty] private string _shelterPhone;
    [ObservableProperty] private string _address;

    [ObservableProperty] private string _errorMessage;
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
                    Email = this.Email,
                    Password = this.Password,
                    FirstName = this.FirstName, 
                    LastName = this.LastName,   
                    Phone = this.Phone,       
                    City = this.City,
                    District = this.District
                };
                await _mediator.Send(command);
            }
            else if (SelectedUserType == "Shelter")
            {
                var command = new RegisterShelterCommand
                {
                    Email = this.Email,
                    Password = this.Password,
                    Name = this.ShelterName,
                    ContactPerson = this.ContactPerson, 
                    Phone = this.ShelterPhone,
                    Address = this.Address              
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