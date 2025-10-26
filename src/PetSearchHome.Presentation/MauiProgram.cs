using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MediatR;
using PetSearchHome.DAL;
using PetSearchHome.BLL;
using PetSearchHome.DAL.Repositories;
using PetSearchHome.BLL.Services;

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

        // PostgreSQL
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connString));

        // DAL
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IListingRepository, ListingRepository>();

        // BLL
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(SomeHandler).Assembly));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IListingService, ListingService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
