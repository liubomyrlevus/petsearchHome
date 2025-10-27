using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace PetSearchHome.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBllServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        return services;
    }
}
