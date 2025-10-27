using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        builder.UseMauiApp<App>();
        ConfigureFonts(builder);
        ConfigureConfiguration(builder);
        ConfigureServices(builder);
        ConfigureLogging(builder);

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

    private static void ConfigureConfiguration(MauiAppBuilder builder)
    {
        builder.Configuration.AddJsonFile(
            path: "appsettings.json",
            optional: false,
            reloadOnChange: true);
    }

    private static void ConfigureServices(MauiAppBuilder builder)
    {
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();
        builder.Services.AddBllServices();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif
    }

    private static void ConfigureLogging(MauiAppBuilder builder)
    {
#if DEBUG
        builder.Logging.AddDebug();
#endif
    }
}