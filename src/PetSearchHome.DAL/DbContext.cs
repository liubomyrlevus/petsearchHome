using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PetSearchHome.BLL.Domain.Entities;
using PetSearchHome.BLL.Domain.Enums;

namespace PetSearchHome.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<RegisteredUser> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<IndividualProfile> IndividualProfiles { get; set; }
    public DbSet<ShelterProfile> ShelterProfiles { get; set; }
    public DbSet<Listing> Listings { get; set; }
    public DbSet<ListingPhoto> ListingPhotos { get; set; }
    public DbSet<HealthInfo> HealthInfos { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // RegisteredUser
        modelBuilder.Entity<RegisteredUser>(entity =>
        {
            entity.ToTable("registered_users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("user_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.UserType).HasColumnName("user_type").HasConversion<string>();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.IsAdmin).HasColumnName("is_admin").HasDefaultValue(false);

            entity.HasOne(u => u.IndividualProfile)
                  .WithOne(i => i.User)
                  .HasForeignKey<IndividualProfile>(i => i.UserId);

            entity.HasOne(u => u.ShelterProfile)
                  .WithOne(s => s.User)
                  .HasForeignKey<ShelterProfile>(s => s.UserId);
        });

        // IndividualProfile
        modelBuilder.Entity<IndividualProfile>(entity =>
        {
            entity.ToTable("individuals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("individual_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.District).HasColumnName("district");
            entity.Property(e => e.AdditionalInfo).HasColumnName("additional_info");
            entity.Property(e => e.PhotoUrl).HasColumnName("photo_url");
        });

        // ShelterProfile
        modelBuilder.Entity<ShelterProfile>(entity =>
        {
            entity.ToTable("shelters");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("shelter_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ContactPerson).HasColumnName("contact_person");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.AdditionalPhone).HasColumnName("additional_phone");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FacebookUrl).HasColumnName("facebook_url");
            entity.Property(e => e.InstagramUrl).HasColumnName("instagram_url");
            entity.Property(e => e.WebsiteUrl).HasColumnName("website_url");
            entity.Property(e => e.LogoUrl).HasColumnName("logo_url");
        });

        // Listing
        modelBuilder.Entity<Listing>(entity =>
        {
            entity.ToTable("listings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("listing_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AnimalType).HasColumnName("animal_type").HasConversion<string>();
            entity.Property(e => e.Breed).HasColumnName("breed");
            entity.Property(e => e.AgeMonths).HasColumnName("age_months");
            entity.Property(e => e.Sex).HasColumnName("sex").HasConversion<string>();
            entity.Property(e => e.Size).HasColumnName("size").HasConversion<string>();
            entity.Property(e => e.Color).HasColumnName("color");
            entity.Property(e => e.City).HasColumnName("city");
            entity.Property(e => e.District).HasColumnName("district");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.SpecialNeeds).HasColumnName("special_needs");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(e => e.ViewsCount).HasColumnName("views_count");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.ModerationComment).HasColumnName("moderation_comment");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId);
        });

        // ListingPhoto
        modelBuilder.Entity<ListingPhoto>(entity =>
        {
            entity.ToTable("photos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("photo_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.ListingId).HasColumnName("listing_id");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");

            entity.HasOne(e => e.Listing)
                  .WithMany(l => l.Photos)
                  .HasForeignKey(e => e.ListingId);
        });

        // HealthInfo
        modelBuilder.Entity<HealthInfo>(entity =>
        {
            entity.ToTable("health_info");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("health_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.ListingId).HasColumnName("listing_id");
            entity.Property(e => e.Vaccinations).HasColumnName("vaccinations");
            entity.Property(e => e.Sterilized).HasColumnName("sterilized");
            entity.Property(e => e.ChronicDiseases).HasColumnName("chronic_diseases");
            entity.Property(e => e.TreatmentHistory).HasColumnName("treatment_history");

            entity.HasOne(e => e.Listing)
                  .WithOne(l => l.HealthInfo)
                  .HasForeignKey<HealthInfo>(e => e.ListingId);
        });

        // Favorite
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.ToTable("favorites");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("favorite_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ListingId).HasColumnName("listing_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            entity.HasIndex(f => new { f.UserId, f.ListingId }).IsUnique();
        });

        // Conversation
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("conversations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("conversation_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.User1Id).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasColumnName("user2_id");
            entity.Property(e => e.ListingId).HasColumnName("listing_id");
            entity.Property(e => e.LastMessageAt).HasColumnName("last_message_at").HasDefaultValueSql("NOW()");

            entity.HasOne(c => c.User1).WithMany().HasForeignKey(c => c.User1Id).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.User2).WithMany().HasForeignKey(c => c.User2Id).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.Listing).WithMany().HasForeignKey(c => c.ListingId).OnDelete(DeleteBehavior.SetNull);
        });

        // Message
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("message_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.IsRead).HasColumnName("is_read").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            entity.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(m => m.Conversation).WithMany(c => c.Messages).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
        });

        // Review
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("reviews");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("review_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.ReviewerId).HasColumnName("reviewer_id");
            entity.Property(e => e.ReviewedId).HasColumnName("reviewed_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.IsModerated).HasColumnName("is_moderated").HasDefaultValue(false);

            entity.HasOne(r => r.Reviewer).WithMany().HasForeignKey(r => r.ReviewerId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(r => r.Reviewed).WithMany().HasForeignKey(r => r.ReviewedId).OnDelete(DeleteBehavior.Cascade);
        });

        // Report
        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("reports");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("report_id").UseIdentityByDefaultColumn();
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.ReportedType).HasColumnName("reported_type").HasConversion<string>();
            entity.Property(e => e.ReportedEntityId).HasColumnName("reported_id");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasDefaultValue(ReportStatus.pending);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");

            entity.HasOne(r => r.Reporter).WithMany().HasForeignKey(r => r.ReporterId).OnDelete(DeleteBehavior.Cascade);
        });

        // Session
        modelBuilder.Entity<Session>(entity =>
        {
            entity.ToTable("sessions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("session_id").HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.SessionToken).HasColumnName("session_token");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.LastActivity).HasColumnName("last_activity").HasDefaultValueSql("NOW()");
            entity.Property(e => e.IsValid).HasColumnName("is_valid").HasDefaultValue(true);

            entity.HasOne(s => s.User)
                  .WithMany()
                  .HasForeignKey(s => s.UserId);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimesToUtc();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        NormalizeDateTimesToUtc();
        return base.SaveChanges();
    }

    private void NormalizeDateTimesToUtc()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not EntityState.Added and not EntityState.Modified)
            {
                continue;
            }

            foreach (var property in entry.Properties)
            {
                var clrType = property.Metadata.ClrType;

                if (clrType == typeof(DateTime) && property.CurrentValue is DateTime dt && dt.Kind == DateTimeKind.Unspecified)
                {
                    property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                }
                else if (clrType == typeof(DateTime?) && property.CurrentValue is DateTime ndt && ndt.Kind == DateTimeKind.Unspecified)
                {
                    property.CurrentValue = (DateTime?)DateTime.SpecifyKind(ndt, DateTimeKind.Utc);
                }
            }
        }
    }
}
