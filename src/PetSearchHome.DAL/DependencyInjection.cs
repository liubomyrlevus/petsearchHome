using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetSearchHome.BLL.Contracts.Persistence;

namespace PetSearchHome.DAL;

public static class DependencyInjection
{
 public static IServiceCollection AddDalServices(this IServiceCollection services, string connectionString)
 {
 services.AddDbContext<ApplicationDbContext>(options =>
 {
 options.UseNpgsql(connectionString);
 });

 // Repositories and UoW
 services.AddScoped<IUnitOfWork, Repositories.UnitOfWork>();
 services.AddScoped<IUserRepository, Repositories.UserRepository>();
 services.AddScoped<IListingRepository, Repositories.ListingRepository>();
 services.AddScoped<IFavoriteRepository, Repositories.FavoriteRepository>();
 services.AddScoped<IConversationRepository, Repositories.ConversationRepository>();
 services.AddScoped<IMessageRepository, Repositories.MessageRepository>();
 services.AddScoped<IReviewRepository, Repositories.ReviewRepository>();
 services.AddScoped<IReportRepository, Repositories.ReportRepository>();
 services.AddScoped<ISessionRepository, Repositories.SessionRepository>();

 return services;
 }
}
