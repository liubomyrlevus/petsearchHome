using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PetSearchHome.BLL.Services.Authentication;

namespace PetSearchHome.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBllServices(this IServiceCollection services, JwtSettings? jwtSettings = null)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        // Business/auth services implemented in BLL
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        if (jwtSettings is not null)
        {
            services.AddSingleton(jwtSettings);
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        }
        else
        {
            // If Presentation will provide IJwtTokenGenerator, skip registration.
        }

        // Register other BLL services (example)
        // services.AddScoped<IUserService, UserService>();

        return services;
    }
}
