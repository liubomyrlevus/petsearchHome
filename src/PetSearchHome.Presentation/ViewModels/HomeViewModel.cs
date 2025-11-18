using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.Presentation.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetSearchHome.ViewModels;

public partial class HomeViewModel : ObservableValidator
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;
    private HashSet<int> _favoriteListingIds = new();

    [ObservableProperty] private string _searchTerm;
    [ObservableProperty] private AnimalType? _selectedAnimalType;
    [ObservableProperty] private string _city;
    [ObservableProperty] private ObservableCollection<ListingCardDto> _listings = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage;

    public HomeViewModel(IMediator mediator, CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [RelayCommand]
    private async Task LoadListingsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var query = new SearchListingsQuery
            {
                SearchQuery = this.SearchTerm,
                AnimalType = this.SelectedAnimalType,
                City = this.City
            };

            var result = await _mediator.Send(query);

            Listings.Clear();
            foreach (var listing in result)
            {
                Listings.Add(listing);
            }

            await LoadFavoritesInternalAsync();
        }
        catch (Exception ex)
        {
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
            ErrorMessage = "Щоб додати в \"Улюблені\", увійдіть у систему.";
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
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Помилка: {ex.Message}";
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
        }
        catch
        {
            // Ігноруємо помилки завантаження улюблених.
        }
    }
}

