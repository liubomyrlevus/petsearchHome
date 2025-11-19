using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSearchHome.ViewModels;

public partial class EditListingViewModel : ObservableValidator
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;
    private readonly CurrentUserService _currentUserService;

    [ObservableProperty]
    private int _listingId;

    [ObservableProperty]
    private AnimalType _animalType = AnimalType.unknown;

    [ObservableProperty]
    private string? _breed;

    [ObservableProperty]
    private int? _ageMonths;

    [ObservableProperty]
    private AnimalSex? _sex = AnimalSex.unknown;

    [ObservableProperty]
    private AnimalSize? _size = AnimalSize.unknown;

    [ObservableProperty]
    private string? _color;

    [ObservableProperty]
    private string? _city;

    [ObservableProperty]
    private string? _district;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _specialNeeds;

    [ObservableProperty]
    private string? _vaccinations;

    [ObservableProperty]
    private bool _sterilized;

    [ObservableProperty]
    private string? _chronicDiseases;

    [ObservableProperty]
    private string? _treatmentHistory;

    [ObservableProperty]
    private string? _mainPhotoUrl;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool CanSubmit =>
        !IsBusy &&
        ListingId > 0 &&
        AnimalType != AnimalType.unknown &&
        !string.IsNullOrWhiteSpace(City) &&
        !string.IsNullOrWhiteSpace(Description);

    public EditListingViewModel(
        IMediator mediator,
        NavigationManager navigationManager,
        CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
        _currentUserService = currentUserService;
    }

    [RelayCommand]
    private async Task LoadAsync(int listingId)
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = string.Empty;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб редагувати оголошення, увійдіть у систему.";
            return;
        }

        IsBusy = true;

        try
        {
            ListingId = listingId;

            var listing = await _mediator.Send(new GetListingDetailsQuery { Id = listingId });

            if (listing.UserId != _currentUserService.UserId)
            {
                ErrorMessage = "Ви не маєте права редагувати це оголошення.";
                return;
            }

            AnimalType = listing.AnimalType;
            Breed = listing.Breed;
            AgeMonths = listing.AgeMonths;
            Sex = listing.Sex ?? AnimalSex.unknown;
            Size = listing.Size ?? AnimalSize.unknown;
            Color = listing.Color;
            City = listing.City;
            District = listing.District;
            Description = listing.Description;
            SpecialNeeds = listing.SpecialNeeds;

            if (listing.HealthInfo is not null)
            {
                Vaccinations = listing.HealthInfo.Vaccinations;
                Sterilized = listing.HealthInfo.Sterilized ?? false;
                ChronicDiseases = listing.HealthInfo.ChronicDiseases;
                TreatmentHistory = listing.HealthInfo.TreatmentHistory;
            }

            MainPhotoUrl = listing.PhotoUrls.Count > 0 ? listing.PhotoUrls[0] : null;
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
    private async Task UpdateListingAsync()
    {
        ErrorMessage = string.Empty;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб редагувати оголошення, увійдіть у систему.";
            return;
        }

        if (ListingId <= 0)
        {
            ErrorMessage = "Оголошення не знайдено.";
            return;
        }

        if (IsBusy)
        {
            return;
        }

        IsBusy = true;

        try
        {
            var command = new UpdateListingCommand
            {
                Id = ListingId,
                UserId = _currentUserService.UserId!.Value,
                AnimalType = AnimalType,
                Breed = Breed ?? string.Empty,
                AgeMonths = AgeMonths ?? 0,
                Sex = Sex ?? AnimalSex.unknown,
                Size = Size ?? AnimalSize.unknown,
                Color = Color ?? string.Empty,
                City = City ?? string.Empty,
                District = District ?? string.Empty,
                Description = Description ?? string.Empty,
                SpecialNeeds = SpecialNeeds,
                HealthInfo = new HealthInfoDto
                {
                    Vaccinations = Vaccinations,
                    Sterilized = Sterilized,
                    ChronicDiseases = ChronicDiseases,
                    TreatmentHistory = TreatmentHistory
                },
                PhotoUrls = string.IsNullOrWhiteSpace(MainPhotoUrl)
                    ? new List<string>()
                    : new List<string> { MainPhotoUrl }
            };

            await _mediator.Send(command);

            _navigationManager.NavigateTo($"/listing/{ListingId}", replace: true);
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

