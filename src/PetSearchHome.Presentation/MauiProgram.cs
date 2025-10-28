using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using MudBlazor.Services;
using PetSearchHome.BLL;

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

// Local shim to satisfy calls to AddBllServices when the actual PetSearchHome.BLL assembly is not referenced.
// Remove this shim after adding a proper reference to the BLL project/package.
public static class BllServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        // No-op placeholder. Register real BLL services here once the BLL assembly is available:
        // Example (when BLL exists):
        // services.AddTransient<IMyBllService, MyBllService>();
        return services;
    }
}