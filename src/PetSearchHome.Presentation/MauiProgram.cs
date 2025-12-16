using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using PetSearchHome.BLL;
using PetSearchHome.BLL.Services.Authentication;
using PetSearchHome.DAL;
using PetSearchHome.Presentation.Components.Pages;
using PetSearchHome.Presentation.Services;
using PetSearchHome.ViewModels;
using Serilog;
using Microsoft.Maui.Storage;
using System.IO;

namespace PetSearchHome.Presentation;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        ConfigureFonts(builder);
        ConfigureConfiguration(builder);

        ConfigureLogging(builder);

        ConfigureServices(builder.Services, builder.Configuration);

        return builder.Build();
    }

    private static void ConfigureFonts(MauiAppBuilder builder)
    {
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        });
    }

    private static void ConfigureConfiguration(MauiAppBuilder builder)
    {
        builder.Configuration.AddJsonFile(
            path: "appsettings.json",
            optional: false,
            reloadOnChange: true);

        builder.Configuration.AddUserSecrets<App>();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMauiBlazorWebView();
        services.AddMudServices();

        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);

        services.AddSingleton(jwtSettings);

        services.AddDalServices(configuration);
        services.AddBllServices();

        services.AddSingleton<CurrentUserService>();

        services.AddTransient(sp => new LoginViewModel(
            sp.GetRequiredService<IMediator>(),
            sp.GetRequiredService<NavigationManager>(),
            sp.GetRequiredService<CurrentUserService>()
        ));
        services.AddTransient(sp => new RegisterViewModel(
            sp.GetRequiredService<IMediator>(),
            sp.GetRequiredService<NavigationManager>(),
            sp.GetRequiredService<CurrentUserService>()
        ));
        services.AddTransient<HomeViewModel>();
        services.AddTransient<CreateListingViewModel>();
        services.AddTransient<EditListingViewModel>();
        services.AddTransient<ListingDetailsViewModel>();
        services.AddTransient<FavoritesViewModel>();
        services.AddTransient<UserProfileViewModel>();
        services.AddTransient<MyListingsViewModel>();
        services.AddTransient<AdminPanelViewModel>();

        services.AddTransient<LoginPage>();
        services.AddTransient<Home>();
        services.AddTransient<RegisterPage>();
        services.AddTransient<CreateListingPage>();
        services.AddTransient<EditListingPage>();
        services.AddTransient<ListingDetails>();
        services.AddTransient<Favorites>();
        services.AddTransient<UserProfile>();
        services.AddTransient<MyListings>();
        services.AddTransient<AdminPanel>();

#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif
    }

    private static void ConfigureLogging(MauiAppBuilder builder)
    {
        var logDir = Path.Combine(FileSystem.AppDataDirectory, "Logs");
        Directory.CreateDirectory(logDir);
        var logFile = Path.Combine(logDir, "app-.log");

        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .WriteTo.File(
                path: logFile,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                shared: true,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            .WriteTo.Debug();

#if DEBUG
        loggerConfiguration.MinimumLevel.Debug();
#else
        loggerConfiguration.MinimumLevel.Information();
#endif

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);

#if DEBUG
        builder.Logging.AddDebug();
#endif
    }
}

