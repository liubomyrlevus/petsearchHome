using Microsoft.EntityFrameworkCore;
using PetSearchHome.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


public interface IRepository<T, TKey> where T : class
{
    Task<T?> GetByIdAsync(TKey id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}

public class Repository<T, TKey> : IRepository<T, TKey> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(TKey id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}

// ---------- SPECIFIC INTERFACES ----------

public interface IUserRepository : IRepository<RegisteredUser, int>
{
    Task<RegisteredUser?> GetByEmailAsync(string email);
    Task<RegisteredUser?> GetUserWithDetailsAsync(int id);
}

public interface ISessionRepository : IRepository<Session, Guid>
{
    Task<Session?> GetByTokenAsync(string token);
}

public interface IIndividualRepository : IRepository<Individual, int>
{
    Task<Individual?> GetByUserIdAsync(int userId);
}

public interface IShelterRepository : IRepository<Shelter, int>
{
    Task<Shelter?> GetByUserIdAsync(int userId);
}

public interface IListingRepository : IRepository<Listing, int>
{
    Task<IEnumerable<Listing>> GetActiveListingsWithDetailsAsync();
    Task<IEnumerable<Listing>> GetListingsByUserIdAsync(int userId);
}

public interface IPhotoRepository : IRepository<Photo, int>
{
    Task<IEnumerable<Photo>> GetPhotosByListingIdAsync(int listingId);
}

public interface IHealthInfoRepository : IRepository<HealthInfo, int>
{
    Task<HealthInfo?> GetByListingIdAsync(int listingId);
}

public interface IFavoriteRepository : IRepository<Favorite, int>
{
    Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId);
    Task<Favorite?> GetFavoriteByUserAndListingAsync(int userId, int listingId);
}

public interface IConversationRepository : IRepository<Conversation, int>
{
    Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(int userId);
}

public interface IMessageRepository : IRepository<Message, int>
{
    Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId);
}

public interface IReviewRepository : IRepository<Review, int>
{
    Task<IEnumerable<Review>> GetReviewsForUserAsync(int reviewedUserId);
}

public interface IReportRepository : IRepository<Report, int>
{
    Task<IEnumerable<Report>> GetPendingReportsAsync();
}



public class UserRepository : Repository<RegisteredUser, int>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RegisteredUser?> GetByEmailAsync(string email)
    {
        return await _context.RegisteredUsers
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<RegisteredUser?> GetUserWithDetailsAsync(int id)
    {
        return await _context.RegisteredUsers
            .Include(u => u.Individual)
            .Include(u => u.Shelter)
            .FirstOrDefaultAsync(u => u.UserId == id);
    }

    public override async Task<RegisteredUser?> GetByIdAsync(int id)
    {
        return await GetUserWithDetailsAsync(id);
    }
}

public class SessionRepository : Repository<Session, Guid>, ISessionRepository
{
    public SessionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Session?> GetByTokenAsync(string token)
    {
        return await _context.Sessions.FirstOrDefaultAsync(s => s.SessionToken == token);
    }
}

public class IndividualRepository : Repository<Individual, int>, IIndividualRepository
{
    public IndividualRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Individual?> GetByUserIdAsync(int userId)
    {
        return await _context.Individuals.FirstOrDefaultAsync(i => i.UserId == userId);
    }
}

public class ShelterRepository : Repository<Shelter, int>, IShelterRepository
{
    public ShelterRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Shelter?> GetByUserIdAsync(int userId)
    {
        return await _context.Shelters.FirstOrDefaultAsync(s => s.UserId == userId);
    }
}

public class ListingRepository : Repository<Listing, int>, IListingRepository
{
    public ListingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Listing>> GetActiveListingsWithDetailsAsync()
    {
        return await _context.Listings
            .Include(l => l.Photos)
            .Include(l => l.HealthInfo)
            .Where(l => l.Status == ListingStatus.active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Listing>> GetListingsByUserIdAsync(int userId)
    {
        return await _context.Listings
            .Include(l => l.Photos)
            .Where(l => l.UserId == userId)
            .ToListAsync();
    }
}

public class PhotoRepository : Repository<Photo, int>, IPhotoRepository
{
    public PhotoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Photo>> GetPhotosByListingIdAsync(int listingId)
    {
        return await _context.Photos
            .Where(p => p.ListingId == listingId)
            .ToListAsync();
    }
}

public class HealthInfoRepository : Repository<HealthInfo, int>, IHealthInfoRepository
{
    public HealthInfoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<HealthInfo?> GetByListingIdAsync(int listingId)
    {
        return await _context.HealthInfos.FirstOrDefaultAsync(h => h.ListingId == listingId);
    }
}

public class FavoriteRepository : Repository<Favorite, int>, IFavoriteRepository
{
    public FavoriteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Favorite>> GetFavoritesByUserIdAsync(int userId)
    {
        return await _context.Favorites
            .Include(f => f.Listing)
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

    public async Task<Favorite?> GetFavoriteByUserAndListingAsync(int userId, int listingId)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId);
    }
}

public class ConversationRepository : Repository<Conversation, int>, IConversationRepository
{
    public ConversationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Conversation>> GetConversationsByUserIdAsync(int userId)
    {
        return await _context.Conversations
            .Include(c => c.User1)
            .Include(c => c.User2)
            .Include(c => c.Messages)
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();
    }
}

public class MessageRepository : Repository<Message, int>, IMessageRepository
{
    public MessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}

public class ReviewRepository : Repository<Review, int>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Review>> GetReviewsForUserAsync(int reviewedUserId)
    {
        return await _context.Reviews
            .Include(r => r.Reviewer)
            .Where(r => r.ReviewedId == reviewedUserId && r.IsModerated)
            .ToListAsync();
    }
}

public class ReportRepository : Repository<Report, int>, IReportRepository
{
    public ReportRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Report>> GetPendingReportsAsync()
    {
        return await _context.Reports
            .Include(r => r.Reporter)
            .Where(r => r.Status == ReportStatus.pending)
            .ToListAsync();
    }
}

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ISessionRepository Sessions { get; }
    IIndividualRepository Individuals { get; }
    IShelterRepository Shelters { get; }
    IListingRepository Listings { get; }
    IPhotoRepository Photos { get; }
    IHealthInfoRepository HealthInfos { get; }
    IFavoriteRepository Favorites { get; }
    IConversationRepository Conversations { get; }
    IMessageRepository Messages { get; }
    IReviewRepository Reviews { get; }
    IReportRepository Reports { get; }

    Task<int> CompleteAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IUserRepository Users { get; private set; }
    public ISessionRepository Sessions { get; private set; }
    public IIndividualRepository Individuals { get; private set; }
    public IShelterRepository Shelters { get; private set; }
    public IListingRepository Listings { get; private set; }
    public IPhotoRepository Photos { get; private set; }
    public IHealthInfoRepository HealthInfos { get; private set; }
    public IFavoriteRepository Favorites { get; private set; }
    public IConversationRepository Conversations { get; private set; }
    public IMessageRepository Messages { get; private set; }
    public IReviewRepository Reviews { get; private set; }
    public IReportRepository Reports { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Sessions = new SessionRepository(_context);
        Individuals = new IndividualRepository(_context);
        Shelters = new ShelterRepository(_context);
        Listings = new ListingRepository(_context);
        Photos = new PhotoRepository(_context);
        HealthInfos = new HealthInfoRepository(_context);
        Favorites = new FavoriteRepository(_context);
        Conversations = new ConversationRepository(_context);
        Messages = new MessageRepository(_context);
        Reviews = new ReviewRepository(_context);
        Reports = new ReportRepository(_context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}