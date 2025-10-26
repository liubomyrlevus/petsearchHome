
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
// Додано using для сервісів MudBlazor
using MudBlazor.Services;
using PetSearchHome.BLL;
//using PetSearchHome.BLL.Services;
using PetSearchHome.DAL;
//using PetSearchHome.DAL.Repositories;
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

        //// PostgreSQL
        //builder.Services.AddDbContext<AppDbContext>(options =>
        //    options.UseNpgsql(connString));

        //// DAL
        //builder.Services.AddScoped<IUserRepository, UserRepository>();
        //builder.Services.AddScoped<IListingRepository, ListingRepository>();

        //// BLL
        //builder.Services.AddMediatR(cfg =>
        //    cfg.RegisterServicesFromAssembly(typeof(SomeHandler).Assembly));
        //builder.Services.AddScoped<IUserService, UserService>();
        //builder.Services.AddScoped<IListingService, ListingService>();

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