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
using System.Linq;

namespace PetSearchHome.ViewModels;


public partial class HomeViewModel : ObservableValidator
{
    private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;

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
            foreach (var listing in result) { Listings.Add(listing); }
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


    [RelayCommand]
    private async Task AddFavoriteAsync(int listingId)
    {
        if (!_currentUserService.IsLoggedIn)
        {
            ErrorMessage = "Щоб додати в улюблені, потрібно увійти в систему.";
            return;
        }

        try
        {
            var command = new AddFavoriteCommand
            {
                UserId = _currentUserService.UserId.Value,
                ListingId = listingId
            };
            await _mediator.Send(command);

           
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Помилка: {ex.Message}";
        }
    }
}