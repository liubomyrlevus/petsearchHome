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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PetSearchHome.ViewModels;

public partial class MyListingsViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;
    private readonly ILogger<MyListingsViewModel> _logger;

    [ObservableProperty] private ObservableCollection<ListingCardDto> _listings = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _infoMessage = string.Empty;

    public MyListingsViewModel(IMediator mediator, CurrentUserService currentUserService, ILogger<MyListingsViewModel> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task LoadListingsAsync()
    {
        if (IsBusy) return;

        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб переглянути власні оголошення, увійдіть до системи.";
            _logger.LogWarning("Load my listings attempted without login");
            return;
        }

        IsBusy = true;

        try
        {
            _logger.LogInformation("Loading my listings for UserId {UserId}", _currentUserService.UserId);
            var listings = await _mediator.Send(new SearchListingsQuery
            {
                UserId = _currentUserService.UserId!.Value,
                Status = null
            });

            Listings = new ObservableCollection<ListingCardDto>(listings);
            _logger.LogInformation("Loaded {Count} listings for UserId {UserId}", Listings.Count, _currentUserService.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading my listings for UserId {UserId}", _currentUserService.UserId);
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteListingAsync(int listingId)
    {
        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Спершу авторизуйтеся, щоб керувати оголошеннями.";
            _logger.LogWarning("Delete listing attempted without login for ListingId {ListingId}", listingId);
            return;
        }

        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        try
        {
            _logger.LogInformation("Deleting listing {ListingId} by UserId {UserId}", listingId, _currentUserService.UserId);
            await _mediator.Send(new DeleteListingCommand
            {
                ListingId = listingId,
                UserId = _currentUserService.UserId!.Value
            });

            var listing = Listings.FirstOrDefault(l => l.Id == listingId);
            if (listing != null)
            {
                Listings.Remove(listing);
            }

            InfoMessage = "Оголошення видалено.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting listing {ListingId} by UserId {UserId}", listingId, _currentUserService.UserId);
            ErrorMessage = ex.Message;
        }
    }

    public string GetStatusLabel(ListingStatus status) => status.ToUkrainianLabel();

    public string GetStatusCss(ListingStatus status) =>
        status switch
        {
            ListingStatus.active => "bg-success",
            ListingStatus.pending => "bg-warning text-dark",
            ListingStatus.rejected => "bg-danger",
            ListingStatus.archived => "bg-secondary",
            _ => "bg-light text-dark"
        };
}
