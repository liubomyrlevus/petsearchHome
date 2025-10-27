using Microsoft.EntityFrameworkCore;
using PetSearchHome.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RegisteredUser> RegisteredUsers { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Individual> Individuals { get; set; }
    public DbSet<Shelter> Shelters { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<HealthInfo> HealthInfos { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<RegisteredUser>().ToTable("registered_users");
        modelBuilder.Entity<Session>().ToTable("sessions");
        modelBuilder.Entity<Individual>().ToTable("individuals");
        modelBuilder.Entity<Shelter>().ToTable("shelters");
        modelBuilder.Entity<Listing>().ToTable("listings");
        modelBuilder.Entity<Photo>().ToTable("photos");
        modelBuilder.Entity<HealthInfo>().ToTable("health_info");
        modelBuilder.Entity<Favorite>().ToTable("favorites");
        modelBuilder.Entity<Conversation>().ToTable("conversations");
        modelBuilder.Entity<Message>().ToTable("messages");
        modelBuilder.Entity<Review>().ToTable("reviews");
        modelBuilder.Entity<Report>().ToTable("reports");

        modelBuilder.Entity<RegisteredUser>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.UserType).HasConversion<string>();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.Property(u => u.IsAdmin).HasDefaultValue(false);

            entity.HasOne(u => u.Individual)
                  .WithOne(i => i.User)
                  .HasForeignKey<Individual>(i => i.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(u => u.Shelter)
                  .WithOne(s => s.User)
                  .HasForeignKey<Shelter>(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(s => s.SessionId).HasDefaultValueSql("gen_random_uuid()");
            entity.HasIndex(s => s.SessionToken).IsUnique();
            entity.Property(s => s.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(s => s.LastActivity).HasDefaultValueSql("NOW()");
            entity.Property(s => s.IsValid).HasDefaultValue(true);

            entity.HasOne(s => s.User)
                  .WithMany(u => u.Sessions)
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Shelter>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<Listing>(entity =>
        {
            entity.Property(l => l.AnimalType).HasConversion<string>();
            entity.Property(l => l.Sex).HasConversion<string>();
            entity.Property(l => l.Size).HasConversion<string>();
            entity.Property(l => l.Status).HasConversion<string>();

            entity.Property(l => l.Status).HasDefaultValue(ListingStatus.draft);
            entity.Property(l => l.ViewsCount).HasDefaultValue(0);
            entity.Property(l => l.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(l => l.UpdatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(l => l.HealthInfo)
                  .WithOne(h => h.Listing)
                  .HasForeignKey<HealthInfo>(h => h.ListingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.Property(p => p.IsPrimary).HasDefaultValue(false);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasIndex(f => new { f.UserId, f.ListingId }).IsUnique();
            entity.Property(f => f.CreatedAt).HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.Property(c => c.LastMessageAt).HasDefaultValueSql("NOW()");

            entity.HasOne(c => c.User1)
                  .WithMany(u => u.ConversationsAsUser1)
                  .HasForeignKey(c => c.User1Id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User2)
                  .WithMany(u => u.ConversationsAsUser2)
                  .HasForeignKey(c => c.User2Id)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Listing)
                  .WithMany(l => l.Conversations)
                  .HasForeignKey(c => c.ListingId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(m => m.IsRead).HasDefaultValue(false);
            entity.Property(m => m.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(m => m.Sender)
                  .WithMany(u => u.MessagesSent)
                  .HasForeignKey(m => m.SenderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");
            entity.Property(r => r.IsModerated).HasDefaultValue(false);

            entity.HasOne(r => r.Reviewer)
                  .WithMany(u => u.ReviewsGiven)
                  .HasForeignKey(r => r.ReviewerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.Reviewed)
                  .WithMany(u => u.ReviewsReceived)
                  .HasForeignKey(r => r.ReviewedId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.Property(r => r.ReportedType).HasConversion<string>();
            entity.Property(r => r.Status).HasConversion<string>();

            entity.Property(r => r.Status).HasDefaultValue(ReportStatus.pending);
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(r => r.Reporter)
                  .WithMany(u => u.ReportsSent)
                  .HasForeignKey(r => r.ReporterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}