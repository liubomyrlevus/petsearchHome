using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PetSearchHome.ViewModels;

public partial class UserProfileViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;
    private readonly ILogger<UserProfileViewModel> _logger;

    [ObservableProperty] private UserProfileDto? _profile;
    [ObservableProperty] private ContactFormModel _contactForm = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;
    [ObservableProperty] private ObservableCollection<ReviewDto> _reviews = new();
    [ObservableProperty] private int _newReviewRating =5;
    [ObservableProperty] private string _newReviewComment = string.Empty;
    [ObservableProperty] private string _reviewErrorMessage = string.Empty;
    [ObservableProperty] private bool _isReviewBusy;

    public bool IsIndividual => Profile?.UserType == UserType.individual;
    public bool IsShelter => Profile?.UserType == UserType.shelter;
    public bool IsOwnProfile =>
        Profile is not null &&
        _currentUserService.IsLoggedIn &&
        _currentUserService.UserId == Profile.Id;
    public bool CanLeaveReview =>
        Profile is not null &&
        _currentUserService.IsLoggedIn &&
        _currentUserService.UserId != Profile.Id;

    public UserProfileViewModel(IMediator mediator, CurrentUserService currentUserService, ILogger<UserProfileViewModel> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    partial void OnProfileChanged(UserProfileDto? value)
    {
        OnPropertyChanged(nameof(IsIndividual));
        OnPropertyChanged(nameof(IsShelter));
        OnPropertyChanged(nameof(IsOwnProfile));
        OnPropertyChanged(nameof(CanLeaveReview));
    }

    [RelayCommand]
    private async Task LoadProfileAsync(int? userId)
    {
        if (IsBusy) return;

        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        ReviewErrorMessage = string.Empty;

        if (!userId.HasValue && !_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб переглянути профіль, увійдіть у систему.";
            _logger.LogWarning("LoadProfile attempted without login");
            return;
        }

        IsBusy = true;

        try
        {
            var targetUserId = userId ?? _currentUserService.UserId!.Value;
            _logger.LogInformation("Loading profile for UserId {TargetUserId}", targetUserId);

            var profile = await _mediator.Send(new GetUserProfileQuery
            {
                UserId = targetUserId
            });

            Profile = profile;
            ContactForm = CreateContactForm(profile);

            var reviews = await _mediator.Send(new GetReviewsForUserQuery
            {
                ReviewedUserId = profile.Id
            });

            Reviews = new ObservableCollection<ReviewDto>(reviews);
            _logger.LogInformation("Profile loaded for UserId {TargetUserId} with {ReviewCount} reviews", targetUserId, Reviews.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for UserId {UserId}", userId);
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
            ErrorMessage = "Профіль не завантажено.";
            _logger.LogWarning("SaveContacts called with null Profile");
            return;
        }

        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            _logger.LogInformation("Updating contacts for UserId {UserId}", Profile.Id);
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
            SuccessMessage = "Контактні дані оновлено.";
            _logger.LogInformation("Contacts updated for UserId {UserId}", Profile.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contacts for UserId {UserId}", Profile.Id);
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task CreateReviewAsync()
    {
        if (Profile is null)
        {
            ReviewErrorMessage = "Профіль користувача не завантажено.";
            _logger.LogWarning("CreateReview called with null Profile");
            return;
        }

        if (!_currentUserService.IsLoggedIn)
        {
            ReviewErrorMessage = "Щоб залишити відгук, увійдіть у систему.";
            _logger.LogWarning("CreateReview attempted without login for ReviewedId {ReviewedId}", Profile.Id);
            return;
        }

        if (_currentUserService.UserId == Profile.Id)
        {
            ReviewErrorMessage = "Не можна залишати відгук про власний профіль.";
            _logger.LogWarning("User {UserId} attempted to review own profile", _currentUserService.UserId);
            return;
        }

        if (NewReviewRating <1 || NewReviewRating >5)
        {
            ReviewErrorMessage = "Оцінка має бути від1 до5.";
            _logger.LogWarning("Invalid review rating {Rating} by UserId {UserId}", NewReviewRating, _currentUserService.UserId);
            return;
        }

        if (IsReviewBusy)
        {
            return;
        }

        IsReviewBusy = true;
        ReviewErrorMessage = string.Empty;

        try
        {
            _logger.LogInformation("Creating review: ReviewerId {ReviewerId} -> ReviewedId {ReviewedId} Rating {Rating}", _currentUserService.UserId, Profile.Id, NewReviewRating);
            var command = new CreateReviewCommand
            {
                ReviewerId = _currentUserService.UserId!.Value,
                ReviewedId = Profile.Id,
                Rating = NewReviewRating,
                Comment = string.IsNullOrWhiteSpace(NewReviewComment) ? null : NewReviewComment.Trim()
            };

            var createdReview = await _mediator.Send(command);

            Reviews.Insert(0, createdReview);
            NewReviewComment = string.Empty;
            _logger.LogInformation("Review created: Id {ReviewId}", createdReview.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review by UserId {UserId} for ReviewedId {ReviewedId}", _currentUserService.UserId, Profile.Id);
            ReviewErrorMessage = ex.Message;
        }
        finally
        {
            IsReviewBusy = false;
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

