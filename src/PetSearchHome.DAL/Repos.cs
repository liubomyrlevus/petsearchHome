using Microsoft.EntityFrameworkCore;
using PetSearchHome.BLL.Contracts.Persistence;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace PetSearchHome.DAL.Repositories;


public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public UnitOfWork(ApplicationDbContext context) { _context = context; }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}


public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) { _context = context; }

    public async Task<RegisteredUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.IndividualProfile)
            .Include(u => u.ShelterProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<RegisteredUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.IndividualProfile)
            .Include(u => u.ShelterProfile)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(RegisteredUser user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(RegisteredUser user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }
}

public class ListingRepository : IListingRepository
{
    private readonly ApplicationDbContext _context;
    public ListingRepository(ApplicationDbContext context) { _context = context; }

    public async Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Listings
            .Include(l => l.HealthInfo)
            .Include(l => l.Photos)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Listings
            .Include(l => l.Photos)
            .Where(l => ids.Contains(l.Id))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> SearchAsync(string? searchQuery, AnimalType? animalType, string? city, ListingStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Listings.AsQueryable();
        if (status.HasValue) query = query.Where(l => l.Status == status.Value);
        if (animalType.HasValue) query = query.Where(l => l.AnimalType == animalType.Value);
        if (!string.IsNullOrWhiteSpace(city)) query = query.Where(l => l.City.ToLower().Contains(city.ToLower()));
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(l => l.Breed.ToLower().Contains(searchQuery.ToLower()) || l.Description.ToLower().Contains(searchQuery.ToLower()));
        }
        return await query.Include(l => l.Photos).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<int> AddAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Listings.AddAsync(listing, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken); 
        return entry.Entity.Id;
    }

    public Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default)
    {
        _context.Listings.Update(listing);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _context.Listings.FindAsync(new object[] { id }, cancellationToken);
        if (listing != null) _context.Listings.Remove(listing);
    }

    public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Listings.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Listing>> GetByOwnerAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Listings.Where(l => l.UserId == userId).AsNoTracking().ToListAsync(cancellationToken);
    }
}

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ApplicationDbContext _context;
    public FavoriteRepository(ApplicationDbContext context) { _context = context; }

    public async Task<IReadOnlyList<Favorite>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, int listingId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ListingId == listingId, cancellationToken);
    }

    public async Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        await _context.Favorites.AddAsync(favorite, cancellationToken);
    }

    public async Task RemoveAsync(Guid userId, int listingId, CancellationToken cancellationToken = default)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId, cancellationToken);
        if (favorite != null) _context.Favorites.Remove(favorite);
    }
}

public class ConversationRepository : IConversationRepository
{
    private readonly ApplicationDbContext _context;
    public ConversationRepository(ApplicationDbContext context) { _context = context; }

    public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation?> GetByParticipantsAsync(Guid user1Id, Guid user2Id, int? listingId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c =>
                ((c.User1Id == user1Id && c.User2Id == user2Id) || (c.User1Id == user2Id && c.User2Id == user1Id))
                && c.ListingId == listingId, cancellationToken);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _context.Conversations.AddAsync(conversation, cancellationToken);
    }

    public Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _context.Conversations.Update(conversation);
        return Task.CompletedTask;
    }
}

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;
    public MessageRepository(ApplicationDbContext context) { _context = context; }

    public async Task<IReadOnlyList<Message>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Messages.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await _context.Messages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null && !message.IsRead)
        {
            message.IsRead = true;
        }
    }
}

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;
    public ReviewRepository(ApplicationDbContext context) { _context = context; }

    public async Task<IReadOnlyList<Review>> GetForUserAsync(Guid reviewedUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => r.ReviewedId == reviewedUserId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
    }

    public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Update(review);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FindAsync(new object[] { id }, cancellationToken);
        if (review != null) _context.Reviews.Remove(review);
    }
}

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;
    public ReportRepository(ApplicationDbContext context) { _context = context; }

    public async Task<IReadOnlyList<Report>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Where(r => r.Status == status)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reports.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _context.Reports.AddAsync(report, cancellationToken);
    }

    public Task UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(report);
        return Task.CompletedTask;
    }
}

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;
    public SessionRepository(ApplicationDbContext context) { _context = context; }

    public async Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.SessionToken == token, cancellationToken);
    }

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
    {
        await _context.Sessions.AddAsync(session, cancellationToken);
    }

    public async Task InvalidateAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _context.Sessions.FindAsync(new object[] { sessionId }, cancellationToken);
        if (session != null && session.IsValid)
        {
            session.IsValid = false;
        }
    }

    public Task UpdateAsync(Session session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Update(session);
        return Task.CompletedTask;
    }
}