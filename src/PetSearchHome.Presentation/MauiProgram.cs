using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using MudBlazor.Services;
using PetSearchHome.BLL;
using PetSearchHome.BLL.Services;
using PetSearchHome.DAL;
//using PetSearchHome.DAL.Repositories;
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