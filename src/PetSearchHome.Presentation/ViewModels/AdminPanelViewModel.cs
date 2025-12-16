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

public partial class AdminPanelViewModel : ObservableObject
{
 private readonly IMediator _mediator;
 private readonly CurrentUserService _currentUserService;
 private readonly ILogger<AdminPanelViewModel> _logger;

 [ObservableProperty] private ObservableCollection<ListingModerationDto> _listingsForModeration = new();
 [ObservableProperty] private ObservableCollection<ReportDto> _reports = new();
 [ObservableProperty] private bool _isBusy;
 [ObservableProperty] private string _errorMessage = string.Empty;

 public bool IsAdmin => _currentUserService.IsLoggedIn && _currentUserService.IsAdmin;

 public AdminPanelViewModel(IMediator mediator, CurrentUserService currentUserService, ILogger<AdminPanelViewModel> logger)
 {
 _mediator = mediator;
 _currentUserService = currentUserService;
 _logger = logger;
 }

 [RelayCommand]
 private async Task LoadModerationListingsAsync()
 {
 if (!IsAdmin)
 {
 ErrorMessage = "Потрібні права адміністратора для перегляду модерації.";
 _logger.LogWarning("LoadModeration attempted without admin rights");
 return;
 }

 if (IsBusy) return;
 IsBusy = true;
 ErrorMessage = string.Empty;

 try
 {
 _logger.LogInformation("Loading listings for moderation");
 var result = new ObservableCollection<ListingModerationDto>();

 var drafts = await _mediator.Send(new GetListingsByStatusQuery { Status = ListingStatus.draft });
 foreach (var l in drafts) { result.Add(l); }

 var pending = await _mediator.Send(new GetListingsByStatusQuery { Status = ListingStatus.pending });
 foreach (var l in pending.Where(p => result.All(r => r.Id != p.Id))) { result.Add(l); }

 ListingsForModeration = result;
 _logger.LogInformation("Loaded {Count} items for moderation", ListingsForModeration.Count);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error loading moderation listings");
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
 await ModerateListingAsync(listingId, ListingStatus.active, "Схвалено модератором");
 }

 [RelayCommand]
 private async Task RejectListingAsync(int listingId)
 {
 await ModerateListingAsync(listingId, ListingStatus.rejected, "Відхилено модератором");
 }

 private async Task ModerateListingAsync(int listingId, ListingStatus newStatus, string comment)
 {
 if (!IsAdmin || !_currentUserService.UserId.HasValue)
 {
 ErrorMessage = "Потрібні права адміністратора для модерації.";
 _logger.LogWarning("ModerateListing attempted without admin or userId for ListingId {ListingId}", listingId);
 return;
 }

 try
 {
 _logger.LogInformation("Moderating ListingId {ListingId} -> {NewStatus}", listingId, newStatus);
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
 _logger.LogError(ex, "Error moderating listing {ListingId}", listingId);
 ErrorMessage = ex.Message;
 }
 }

 [RelayCommand]
 private async Task LoadReportsAsync()
 {
 if (!IsAdmin)
 {
 ErrorMessage = "Потрібні права адміністратора для перегляду звітів.";
 _logger.LogWarning("LoadReports attempted without admin rights");
 return;
 }

 if (IsBusy) return;
 IsBusy = true;
 ErrorMessage = string.Empty;

 try
 {
 _logger.LogInformation("Loading pending reports");
 var reports = await _mediator.Send(new GetReportsByStatusQuery { Status = ReportStatus.pending });
 Reports = new ObservableCollection<ReportDto>(reports);
 _logger.LogInformation("Loaded {Count} reports", Reports.Count);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Error loading reports");
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
 ErrorMessage = "Потрібні права адміністратора для змін звіту.";
 _logger.LogWarning("UpdateReportStatus attempted without admin or userId for ReportId {ReportId}", reportId);
 return;
 }

 try
 {
 _logger.LogInformation("Updating ReportId {ReportId} -> {NewStatus}", reportId, newStatus);
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
 _logger.LogError(ex, "Error updating report {ReportId}", reportId);
 ErrorMessage = ex.Message;
 }
 }
}
