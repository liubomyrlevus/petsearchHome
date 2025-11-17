using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.Presentation.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PetSearchHome.ViewModels;

public partial class CreateListingViewModel : ObservableValidator
{
    private readonly IMediator _mediator;
    private readonly NavigationManager _navigationManager;
    private readonly CurrentUserService _currentUserService;

    [ObservableProperty]
    [Required(ErrorMessage = "Тип тварини є обов'язковим")]
    private AnimalType _animalType = AnimalType.unknown;

    [ObservableProperty]
    [Required(ErrorMessage = "Порода є обов'язковою")]
    private string _breed = string.Empty;

    [ObservableProperty]
    [Range(0, 360, ErrorMessage = "Вік має бути від 0 до 360 місяців")]
    private int? _ageMonths;

    [ObservableProperty]
    private AnimalSex? _sex = AnimalSex.unknown;

    [ObservableProperty]
    private AnimalSize? _size = AnimalSize.unknown;

    [ObservableProperty]
    private string? _color;

    [ObservableProperty]
    [Required(ErrorMessage = "Місто є обов'язковим")]
    private string _city = string.Empty;

    [ObservableProperty]
    private string? _district;

    [ObservableProperty]
    [Required(ErrorMessage = "Опис є обов'язковим")]
    [MinLength(10, ErrorMessage = "Опис має містити щонайменше 10 символів")]
    private string _description = string.Empty;

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
        !string.IsNullOrWhiteSpace(Breed) &&
        !string.IsNullOrWhiteSpace(City) &&
        !string.IsNullOrWhiteSpace(Description) &&
        AnimalType != AnimalType.unknown;

    public CreateListingViewModel(
        IMediator mediator,
        NavigationManager navigationManager,
        CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _navigationManager = navigationManager;
        _currentUserService = currentUserService;
    }

    [RelayCommand]
    private async Task CreateListingAsync()
    {
        ErrorMessage = string.Empty;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб створити оголошення, спочатку увійдіть у систему.";
            return;
        }

        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }

        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var command = new CreateListingCommand
            {
                UserId = _currentUserService.UserId!.Value,
                AnimalType = AnimalType,
                Breed = Breed,
                AgeMonths = AgeMonths,
                Sex = Sex,
                Size = Size,
                Color = Color,
                City = City,
                District = District,
                Description = Description,
                SpecialNeeds = SpecialNeeds,
                HealthInfo = new HealthInfoDto
                {
                    Vaccinations = Vaccinations,
                    Sterilized = Sterilized,
                    ChronicDiseases = ChronicDiseases,
                    TreatmentHistory = TreatmentHistory
                },
                PhotoUrls = string.IsNullOrWhiteSpace(MainPhotoUrl)
                    ? new System.Collections.Generic.List<string>()
                    : new System.Collections.Generic.List<string> { MainPhotoUrl }
            };

            var listingId = await _mediator.Send(command);

            // Після успішного створення повертаємося на головну сторінку,
            // де нове оголошення буде доступне в загальному списку.
            _navigationManager.NavigateTo("/home", replace: true);
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
