using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Domain.Enums;
using PetSearchHome.BLL.DTOs;
using PetSearchHome.BLL.Queries;
using PetSearchHome.Presentation.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PetSearchHome.ViewModels;

public partial class MyListingsViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;

    [ObservableProperty] private ObservableCollection<ListingCardDto> _listings = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _infoMessage = string.Empty;

    public MyListingsViewModel(IMediator mediator, CurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
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
            return;
        }

        IsBusy = true;

        try
        {
            var listings = await _mediator.Send(new SearchListingsQuery
            {
                UserId = _currentUserService.UserId!.Value,
                Status = null
            });

            Listings = new ObservableCollection<ListingCardDto>(listings);
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
    private async Task DeleteListingAsync(int listingId)
    {
        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Спершу авторизуйтеся, щоб керувати оголошеннями.";
            return;
        }

        ErrorMessage = string.Empty;
        InfoMessage = string.Empty;

        try
        {
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
