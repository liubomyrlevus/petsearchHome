using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PetSearchHome.ViewModels;

public partial class FavoritesViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;
    private readonly ILogger<FavoritesViewModel> _logger;

    [ObservableProperty] private ObservableCollection<FavoriteListingDto> _favorites = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public FavoritesViewModel(IMediator mediator, CurrentUserService currentUserService, ILogger<FavoritesViewModel> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task LoadFavoritesAsync()
    {
        if (IsBusy) return;

        ErrorMessage = string.Empty;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Увійдіть до облікового запису, щоб переглянути обране.";
            _logger.LogWarning("LoadFavorites attempted without login");
            return;
        }

        IsBusy = true;

        try
        {
            _logger.LogInformation("Loading favorites for UserId {UserId}", _currentUserService.UserId);
            var result = await _mediator.Send(new GetUserFavoritesQuery
            {
                UserId = _currentUserService.UserId!.Value
            });

            Favorites.Clear();
            foreach (var item in result)
            {
                Favorites.Add(item);
            }
            _logger.LogInformation("Loaded {Count} favorites for UserId {UserId}", Favorites.Count, _currentUserService.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading favorites for UserId {UserId}", _currentUserService.UserId);
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RemoveFavoriteAsync(int listingId)
    {
        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Авторизуйтеся, щоб керувати списком обраного.";
            _logger.LogWarning("RemoveFavorite attempted without login for ListingId {ListingId}", listingId);
            return;
        }

        try
        {
            _logger.LogInformation("Removing favorite ListingId {ListingId} for UserId {UserId}", listingId, _currentUserService.UserId);
            await _mediator.Send(new RemoveFavoriteCommand
            {
                UserId = _currentUserService.UserId!.Value,
                ListingId = listingId
            });

            var toRemove = Favorites.FirstOrDefault(f => f.ListingId == listingId);
            if (toRemove != null)
            {
                Favorites.Remove(toRemove);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing favorite ListingId {ListingId} for UserId {UserId}", listingId, _currentUserService.UserId);
            ErrorMessage = ex.Message;
        }
    }
}
