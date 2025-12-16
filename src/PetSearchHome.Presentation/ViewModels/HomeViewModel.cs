using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.DAL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.Presentation.Services;
using PetSearchHome.BLL.Queries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PetSearchHome.ViewModels;

public partial class HomeViewModel : ObservableValidator
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;
    private readonly ILogger<HomeViewModel> _logger;
    private HashSet<int> _favoriteListingIds = new();

    [ObservableProperty] private string _searchTerm = string.Empty;
    [ObservableProperty] private AnimalType? _selectedAnimalType;
    [ObservableProperty] private string _city = string.Empty;
    [ObservableProperty] private ObservableCollection<ListingCardDto> _listings = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public HomeViewModel(IMediator mediator, CurrentUserService currentUserService, ILogger<HomeViewModel> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task LoadListingsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        _logger.LogInformation("Loading listings with Search={Search}, Type={Type}, City={City}", SearchTerm, SelectedAnimalType, City);

        try
        {
            var query = new SearchListingsQuery
            {
                SearchQuery = SearchTerm,
                AnimalType = SelectedAnimalType,
                City = City
            };

            var result = await _mediator.Send(query);

            Listings.Clear();
            foreach (var listing in result)
            {
                Listings.Add(listing);
            }

            _logger.LogInformation("Loaded {Count} listings", Listings.Count);
            await LoadFavoritesInternalAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading listings");
            ErrorMessage = $"Не вдалося завантажити оголошення: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public bool IsFavorite(int listingId) => _favoriteListingIds.Contains(listingId);

    [RelayCommand]
    private async Task ToggleFavoriteAsync(int listingId)
    {
        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб працювати з обраним, спочатку увійдіть у систему.";
            _logger.LogWarning("Toggle favorite attempted without login for ListingId {ListingId}", listingId);
            return;
        }

        try
        {
            var userId = _currentUserService.UserId!.Value;

            if (_favoriteListingIds.Contains(listingId))
            {
                var command = new RemoveFavoriteCommand
                {
                    UserId = userId,
                    ListingId = listingId
                };
                await _mediator.Send(command);
                _favoriteListingIds.Remove(listingId);
                _logger.LogInformation("Removed ListingId {ListingId} from favorites for UserId {UserId}", listingId, userId);
            }
            else
            {
                var command = new AddFavoriteCommand
                {
                    UserId = userId,
                    ListingId = listingId
                };
                await _mediator.Send(command);
                _favoriteListingIds.Add(listingId);
                _logger.LogInformation("Added ListingId {ListingId} to favorites for UserId {UserId}", listingId, userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite ListingId {ListingId}", listingId);
            ErrorMessage = $"Сталася помилка: {ex.Message}";
        }
    }

    private async Task LoadFavoritesInternalAsync()
    {
        if (!_currentUserService.IsLoggedIn)
        {
            _favoriteListingIds = new HashSet<int>();
            return;
        }

        try
        {
            var favorites = await _mediator.Send(new GetUserFavoritesQuery
            {
                UserId = _currentUserService.UserId!.Value
            });

            _favoriteListingIds = new HashSet<int>(favorites.Select(f => f.ListingId));
            _logger.LogInformation("Loaded favorites: {Count}", _favoriteListingIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load favorites, continuing without favorites");
        }
    }
}
