using MediatR;
using Microsoft.AspNetCore.Components;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Features.Auth.DTOs;
using PetSearchHome.Presentation;
using PetSearchHome.ViewModels;

namespace PetSearchHome.Presentation.Components.Pages;

public partial class RegisterPage : ComponentBase
{
 [Inject] private NavigationManager Nav { get; set; } = default!;
 [Inject] private IMediator Mediator { get; set; } = default!;

 protected RegisterViewModel RegisterModel { get; set; } = new();
 protected string? ErrorMessage { get; set; }

 protected async Task HandleRegisterSubmit()
 {
 ErrorMessage = null;
 try
 {
 if (RegisterModel.AccountType == UserType.PrivatePerson)
 {
 // Split FullName into first/last as BLL expects
 var parts = (RegisterModel.FullName ?? string.Empty).Trim().Split(' ',2, StringSplitOptions.RemoveEmptyEntries);
 var firstName = parts.Length >0 ? parts[0] : string.Empty;
 var lastName = parts.Length >1 ? parts[1] : string.Empty;

 var cmd = new RegisterIndividualCommand
 {
 Email = RegisterModel.Email,
 Password = RegisterModel.Password,
 FirstName = firstName,
 LastName = lastName,
 Phone = RegisterModel.Phone,
 City = RegisterModel.Address, // If you store City + District together, you could parse them; for now map Address to City
 District = string.Empty
 };
 var result = await Mediator.Send(cmd);
 await OnRegistered(result);
 }
 else
 {
 var cmd = new RegisterShelterCommand
 {
 Email = RegisterModel.Email,
 Password = RegisterModel.Password,
 Name = RegisterModel.ShelterName,
 ContactPerson = RegisterModel.ContactPerson,
 Phone = RegisterModel.Phone,
 Address = RegisterModel.ShelterAddress
 };
 var result = await Mediator.Send(cmd);
 await OnRegistered(result);
 }
 }
 catch (Exception ex)
 {
 ErrorMessage = ex.Message;
 }
 }

 private Task OnRegistered(PetSearchHome.BLL.DTOs.LoginResultDto result)
 {
 if (result is not null && !string.IsNullOrWhiteSpace(result.Token))
 {
 // TODO: persist token securely (Preferences/SecureStorage) if needed
 Nav.NavigateTo("/home");
 }
 else
 {
 ErrorMessage = "?? ??????? ???????? ??????";
 }
 return Task.CompletedTask;
 }
}
