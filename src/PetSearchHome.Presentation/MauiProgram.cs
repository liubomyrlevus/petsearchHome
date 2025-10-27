
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using PetSearchHome.BLL;
using PetSearchHome.BLL.Services;
using PetSearchHome.DAL;
// using PetSearchHome.DAL.Repositories;
using System;

namespace PetSearchHome.Presentation;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var connString = config.GetConnectionString("DefaultConnection");

        // PostgreSQL and DAL registrations
        // NOTE: AppDbContext, UserRepository and ListingRepository implementations are not present in the DAL project yet.
        // When those are implemented in the DAL, register them here. Example:
        // builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connString));
        // builder.Services.AddScoped<IUserRepository, UserRepository>();
        // builder.Services.AddScoped<IListingRepository, ListingRepository>();

        // BLL - register services (including MediatR handlers) from BLL assembly
        builder.Services.AddBllServices();
        // NOTE: IUserService / IListingService implementations are not present in BLL yet.
        // If/when you add them, register here like:
        // builder.Services.AddScoped<IUserService, UserService>();
        // builder.Services.AddScoped<IListingService, ListingService>();

        // --- Додано сервіси для Blazor та MudBlazor ---

        // 1. Потрібно для запуску Blazor всередині MAUI
        builder.Services.AddMauiBlazorWebView();

        // 2. Потрібно для роботи компонентів MudBlazor
        builder.Services.AddMudServices();

        // ------------------------------------------------

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}