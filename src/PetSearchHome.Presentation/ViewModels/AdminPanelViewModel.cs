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

public partial class AdminPanelViewModel : ObservableObject
{
 private readonly IMediator _mediator;
    private readonly CurrentUserService _currentUserService;

  [ObservableProperty] private ObservableCollection<ListingModerationDto> _listingsForModeration = new();
    [ObservableProperty] private ObservableCollection<ReportDto> _reports = new();
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public bool IsAdmin => _currentUserService.IsLoggedIn && _currentUserService.IsAdmin;

    public AdminPanelViewModel(IMediator mediator, CurrentUserService currentUserService)
    {
  _mediator = mediator;
     _currentUserService = currentUserService;
    }

    [RelayCommand]
  private async Task LoadModerationListingsAsync()
    {
    if (!IsAdmin)
  {
            ErrorMessage = "??????????? ???? ??? ????????? ???? ????????.";
      return;
        }

        if (IsBusy) return;
        IsBusy = true;
   ErrorMessage = string.Empty;

        try
        {
     var result = new ObservableCollection<ListingModerationDto>();

            // ????????? ?????????? ? ??????? Draft
            var drafts = await _mediator.Send(new GetListingsByStatusQuery
            {
          Status = ListingStatus.draft
            });

            foreach (var l in drafts)
        {
                result.Add(l);
    }

            // ????????? ?????????? ? ??????? Pending (ModerationPending)
            var pending = await _mediator.Send(new GetListingsByStatusQuery
            {
  Status = ListingStatus.pending
    });

        foreach (var l in pending.Where(p => result.All(r => r.Id != p.Id)))
         {
           result.Add(l);
   }

     ListingsForModeration = result;
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
    private async Task ApproveListingAsync(int listingId)
    {
     await ModerateListingAsync(listingId, ListingStatus.active, "???????? ???????????");
    }

    [RelayCommand]
    private async Task RejectListingAsync(int listingId)
    {
        await ModerateListingAsync(listingId, ListingStatus.rejected, "????????? ???????????");
    }

    private async Task ModerateListingAsync(int listingId, ListingStatus newStatus, string comment)
    {
        if (!IsAdmin || !_currentUserService.UserId.HasValue)
 {
    ErrorMessage = "??????????? ???? ??? ????????? ?????????.";
         return;
      }

        try
        {
            var command = new ModerateListingCommand
            {
  ListingId = listingId,
     ModeratorId = _currentUserService.UserId.Value,
      NewStatus = newStatus,
           ModerationComment = comment
        };

            await _mediator.Send(command);

 var listing = ListingsForModeration.FirstOrDefault(l => l.Id == listingId);
    if (listing != null)
     {
        ListingsForModeration.Remove(listing);
}
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
      }
    }

    [RelayCommand]
    private async Task LoadReportsAsync()
 {
 if (!IsAdmin)
        {
         ErrorMessage = "??????????? ???? ??? ????????? ?????.";
        return;
        }

        if (IsBusy) return;
        IsBusy = true;
ErrorMessage = string.Empty;

        try
  {
     var reports = await _mediator.Send(new GetReportsByStatusQuery
  {
        Status = ReportStatus.pending
            });

 Reports = new ObservableCollection<ReportDto>(reports);
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
    private async Task ResolveReportAsync(int reportId)
    {
        await UpdateReportStatusInternalAsync(reportId, ReportStatus.confirmed);
    }

  [RelayCommand]
    private async Task RejectReportAsync(int reportId)
    {
        await UpdateReportStatusInternalAsync(reportId, ReportStatus.rejected);
    }

    private async Task UpdateReportStatusInternalAsync(int reportId, ReportStatus newStatus)
    {
        if (!IsAdmin || !_currentUserService.UserId.HasValue)
        {
      ErrorMessage = "??????????? ???? ??? ????????? ?????.";
            return;
        }

      try
    {
    var command = new UpdateReportStatusCommand
            {
        ReportId = reportId,
      NewStatus = newStatus,
       ModeratorId = _currentUserService.UserId.Value
      };

   await _mediator.Send(command);

   var report = Reports.FirstOrDefault(r => r.Id == reportId);
         if (report != null)
       {
         Reports.Remove(report);
          }
        }
        catch (Exception ex)
   {
      ErrorMessage = ex.Message;
        }
    }
}
