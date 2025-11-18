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

namespace PetSearchHome.ViewModels;

public partial class FavoritesViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;

    [ObservableProperty] private ObservableCollection<FavoriteListingDto> _favorites = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public FavoritesViewModel(IMediator mediator, CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [RelayCommand]
    private async Task LoadFavoritesAsync()
    {
        if (IsBusy) return;

        ErrorMessage = string.Empty;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб переглядати \"Улюблені\", увійдіть у систему.";
            return;
        }

        IsBusy = true;

        try
        {
            var result = await _mediator.Send(new GetUserFavoritesQuery
            {
                UserId = _currentUserService.UserId!.Value
            });

            Favorites.Clear();
            foreach (var item in result)
            {
                Favorites.Add(item);
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

    [RelayCommand]
    private async Task RemoveFavoriteAsync(int listingId)
    {
        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб керувати \"Улюбленими\", увійдіть у систему.";
            return;
        }

        try
        {
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
            ErrorMessage = ex.Message;
        }
    }
}

