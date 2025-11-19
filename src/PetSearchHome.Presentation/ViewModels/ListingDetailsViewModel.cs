using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System;
using System.Threading.Tasks;

namespace PetSearchHome.ViewModels;

public partial class ListingDetailsViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;

    [ObservableProperty] private ListingDetailsDto? _listing;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isFavorite;

    public ListingDetailsViewModel(IMediator mediator, CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [RelayCommand]
    private async Task LoadAsync(int listingId)
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            Listing = await _mediator.Send(new GetListingDetailsQuery { Id = listingId });
            IsFavorite = false;
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
    private async Task ToggleFavoriteAsync()
    {
        if (Listing is null) return;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Увійдіть до облікового запису, щоб додати оголошення в обране.";
            return;
        }

        try
        {
            var userId = _currentUserService.UserId!.Value;

            if (IsFavorite)
            {
                await _mediator.Send(new RemoveFavoriteCommand
                {
                    UserId = userId,
                    ListingId = Listing.Id
                });
                IsFavorite = false;
            }
            else
            {
                await _mediator.Send(new AddFavoriteCommand
                {
                    UserId = userId,
                    ListingId = Listing.Id
                });
                IsFavorite = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
