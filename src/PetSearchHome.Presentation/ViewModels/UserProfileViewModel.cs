using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System;
using System.Threading.Tasks;

namespace PetSearchHome.ViewModels;

public partial class UserProfileViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;

    [ObservableProperty] private UserProfileDto? _profile;
    [ObservableProperty] private ContactFormModel _contactForm = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;

    public bool IsIndividual => Profile?.UserType == UserType.individual;
    public bool IsShelter => Profile?.UserType == UserType.shelter;

    public UserProfileViewModel(IMediator mediator, CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    partial void OnProfileChanged(UserProfileDto? value)
    {
        OnPropertyChanged(nameof(IsIndividual));
        OnPropertyChanged(nameof(IsShelter));
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        if (IsBusy) return;

        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб переглянути профіль, авторизуйтеся.";
            return;
        }

        IsBusy = true;

        try
        {
            var profile = await _mediator.Send(new GetUserProfileQuery
            {
                UserId = _currentUserService.UserId!.Value
            });

            Profile = profile;
            ContactForm = CreateContactForm(profile);
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

    [RelayCommand]
    private async Task SaveContactsAsync()
    {
        if (Profile == null)
        {
            ErrorMessage = "Дані профілю відсутні.";
            return;
        }

        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            var command = new UpdateUserContactInfoCommand
            {
                UserId = Profile.Id
            };

            if (Profile.UserType == UserType.individual)
            {
                command.Individual = new IndividualContactInfoUpdate
                {
                    Phone = ContactForm.Phone,
                    City = ContactForm.City,
                    District = ContactForm.District,
                    AdditionalInfo = ContactForm.AdditionalInfo
                };
            }
            else if (Profile.UserType == UserType.shelter)
            {
                command.Shelter = new ShelterContactInfoUpdate
                {
                    ContactPerson = ContactForm.ContactPerson,
                    Phone = ContactForm.Phone,
                    Address = ContactForm.Address,
                    Description = ContactForm.Description
                };
            }

            var updatedProfile = await _mediator.Send(command);

            Profile = updatedProfile;
            ContactForm = CreateContactForm(updatedProfile);
            SuccessMessage = "Контакти успішно збережено.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private static ContactFormModel CreateContactForm(UserProfileDto profile)
    {
        var form = new ContactFormModel();

        if (profile.UserType == UserType.individual && profile.IndividualProfile != null)
        {
            form.Phone = profile.IndividualProfile.Phone;
            form.City = profile.IndividualProfile.City;
            form.District = profile.IndividualProfile.District;
            form.AdditionalInfo = profile.IndividualProfile.AdditionalInfo;
        }
        else if (profile.UserType == UserType.shelter && profile.ShelterProfile != null)
        {
            form.ContactPerson = profile.ShelterProfile.ContactPerson;
            form.Phone = profile.ShelterProfile.Phone;
            form.Address = profile.ShelterProfile.Address;
            form.Description = profile.ShelterProfile.Description;
        }

        return form;
    }
}

public class ContactFormModel
{
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? ContactPerson { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
}
