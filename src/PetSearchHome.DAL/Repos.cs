using Microsoft.EntityFrameworkCore;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetSearchHome.DAL.Repositories;

// --- UNIT OF WORK ---
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public UnitOfWork(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.SaveChangesAsync(cancellationToken);
    }
}

// --- REPOSITORY IMPLEMENTATIONS ---

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public UserRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<RegisteredUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await _context.Users
            .Include(u => u.IndividualProfile)
            .Include(u => u.ShelterProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<RegisteredUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await _context.Users
            .Include(u => u.IndividualProfile)
            .Include(u => u.ShelterProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(RegisteredUser user, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(RegisteredUser user, CancellationToken cancellationToken = default)
    {
        if (user.CreatedAt.Kind == DateTimeKind.Unspecified)
        {
            user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);
        }

        if (user.LastLogin.HasValue && user.LastLogin.Value.Kind == DateTimeKind.Unspecified)
        {
            user.LastLogin = DateTime.SpecifyKind(user.LastLogin.Value, DateTimeKind.Utc);
        }

        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class ListingRepository : IListingRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public ListingRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await _context.Listings
            .Include(l => l.HealthInfo)
            .Include(l => l.Photos)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await _context.Listings
            .Include(l => l.Photos)
            .Where(l => ids.Contains(l.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> SearchAsync(string? searchQuery, AnimalType? animalType, string? city, ListingStatus? status, int? userId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = _context.Listings.AsQueryable();
        if (status.HasValue) query = query.Where(l => l.Status == status.Value);
        if (userId.HasValue) query = query.Where(l => l.UserId == userId.Value);
        if (animalType.HasValue) query = query.Where(l => l.AnimalType == animalType.Value);
        if (!string.IsNullOrWhiteSpace(city)) query = query.Where(l => l.City.ToLower().Contains(city.ToLower()));
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(l =>
                (l.Breed ?? string.Empty).ToLower().Contains(searchQuery.ToLower()) ||
                (l.Description ?? string.Empty).ToLower().Contains(searchQuery.ToLower()));
        }

        return await query.Include(l => l.Photos).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        // Ensure UTC kinds
        if (listing.CreatedAt.Kind == DateTimeKind.Unspecified)
        {
            listing.CreatedAt = DateTime.SpecifyKind(listing.CreatedAt, DateTimeKind.Utc);
        }
        if (listing.UpdatedAt.HasValue && listing.UpdatedAt.Value.Kind == DateTimeKind.Unspecified)
        {
            listing.UpdatedAt = DateTime.SpecifyKind(listing.UpdatedAt.Value, DateTimeKind.Utc);
        }

        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var entry = await _context.Listings.AddAsync(listing, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity.Id;
    }

    public async Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        // Normalize DateTime kinds to UTC to satisfy Npgsql timestamptz requirements
        if (listing.CreatedAt.Kind == DateTimeKind.Unspecified)
        {
            listing.CreatedAt = DateTime.SpecifyKind(listing.CreatedAt, DateTimeKind.Utc);
        }
        if (listing.UpdatedAt.HasValue && listing.UpdatedAt.Value.Kind == DateTimeKind.Unspecified)
        {
            listing.UpdatedAt = DateTime.SpecifyKind(listing.UpdatedAt.Value, DateTimeKind.Utc);
        }

        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        _context.Listings.Update(listing);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var listing = await _context.Listings.FindAsync(new object[] { id }, cancellationToken);
        if (listing != null)
        {
            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Listings.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> GetByOwnerAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Listings
            .Where(l => l.UserId == userId)
            .Include(l => l.Photos)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}

public class FavoriteRepository : IFavoriteRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public FavoriteRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<IReadOnlyList<Favorite>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await _context.Favorites.AddAsync(favorite, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int userId, int listingId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Favorites
            .AnyAsync(f => f.UserId == userId && f.ListingId == listingId, cancellationToken);
    }

    public async Task RemoveAsync(int userId, int listingId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId, cancellationToken);
        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

public class ConversationRepository : IConversationRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public ConversationRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<Conversation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Conversations
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation?> GetByParticipantsAsync(int user1Id, int user2Id, int? listingId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Conversations
            .FirstOrDefaultAsync(c =>
                ((c.User1Id == user1Id && c.User2Id == user2Id) || (c.User1Id == user2Id && c.User2Id == user1Id))
                && c.ListingId == listingId, cancellationToken);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await _context.Conversations.AddAsync(conversation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class MessageRepository : IMessageRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public MessageRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<IReadOnlyList<Message>> GetByConversationAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Messages.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(int messageId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var message = await _context.Messages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null && !message.IsRead)
        {
            message.IsRead = true;
            _context.Messages.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateAsync(Message message, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        _context.Messages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class ReviewRepository : IReviewRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public ReviewRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<IReadOnlyList<Review>> GetForUserAsync(int reviewedUserId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Reviews
            .Where(r => r.ReviewedId == reviewedUserId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Reviews.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await _context.Reviews.AddAsync(review, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var review = await _context.Reviews.FindAsync(new object[] { id }, cancellationToken);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

public class ReportRepository : IReportRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public ReportRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<IReadOnlyList<Report>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Reports
            .Where(r => r.Status == status)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Report?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Reports.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
    {
        // Normalize DateTime kinds to UTC
        if (report.CreatedAt.Kind == DateTimeKind.Unspecified)
        {
            report.CreatedAt = DateTime.SpecifyKind(report.CreatedAt, DateTimeKind.Utc);
        }
        if (report.ResolvedAt.HasValue && report.ResolvedAt.Value.Kind == DateTimeKind.Unspecified)
        {
            report.ResolvedAt = DateTime.SpecifyKind(report.ResolvedAt.Value, DateTimeKind.Utc);
        }

        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await _context.Reports.AddAsync(report, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        // Normalize DateTime kinds to UTC
        if (report.CreatedAt.Kind == DateTimeKind.Unspecified)
        {
            report.CreatedAt = DateTime.SpecifyKind(report.CreatedAt, DateTimeKind.Utc);
        }
        if (report.ResolvedAt.HasValue && report.ResolvedAt.Value.Kind == DateTimeKind.Unspecified)
        {
            report.ResolvedAt = DateTime.SpecifyKind(report.ResolvedAt.Value, DateTimeKind.Utc);
        }

        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        _context.Reports.Update(report);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class SessionRepository : ISessionRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public SessionRepository(IDbContextFactory<ApplicationDbContext> contextFactory) { _contextFactory = contextFactory; }

    public async Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.SessionToken == token, cancellationToken);
    }

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        await _context.Sessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task InvalidateAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var session = await _context.Sessions.FindAsync(new object[] { sessionId }, cancellationToken);
        if (session != null && session.IsValid)
        {
            session.IsValid = false;
            _context.Sessions.Update(session);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task UpdateAsync(Session session, CancellationToken cancellationToken = default)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
