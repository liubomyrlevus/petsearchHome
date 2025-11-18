using Microsoft.Extensions.DependencyInjection;
using PetSearchHome.BLL.Commands;
using PetSearchHome.BLL.Services.Authentication;
using System.Reflection;
using MediatR;

namespace PetSearchHome.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(RegisterIndividualCommand).Assembly);

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

  
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>(provider =>
        {
            var settings = provider.GetRequiredService<JwtSettings>();
            return new JwtTokenGenerator(settings);
        });

        return services;
    }
}