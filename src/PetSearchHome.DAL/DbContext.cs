using Microsoft.EntityFrameworkCore;
using PetSearchHome.BLL.Domain.Entities; 
using PetSearchHome.BLL.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        // Value converters
        var userTypeConverter = new ValueConverter<UserType, string>(
            v => v.ToString().ToLowerInvariant(),
            v => Enum.Parse<UserType>(v, true));

        // registered_users
        modelBuilder.Entity<RegisteredUser>().ToTable("registered_users");
        modelBuilder.Entity<RegisteredUser>().HasKey(u => u.Id);
        modelBuilder.Entity<RegisteredUser>().Property(u => u.Id).HasColumnName("user_id").ValueGeneratedOnAdd();
        modelBuilder.Entity<RegisteredUser>().Property(u => u.Email).HasColumnName("email");
        modelBuilder.Entity<RegisteredUser>().Property(u => u.PasswordHash).HasColumnName("password_hash");
        modelBuilder.Entity<RegisteredUser>()
            .Property(u => u.UserType)
            .HasColumnName("user_type")
            .HasConversion(userTypeConverter);
        modelBuilder.Entity<RegisteredUser>().Property(u => u.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<RegisteredUser>().Property(u => u.LastLogin).HasColumnName("last_login");
        modelBuilder.Entity<RegisteredUser>().Property(u => u.IsActive).HasColumnName("is_active");
        modelBuilder.Entity<RegisteredUser>().Property(u => u.IsAdmin).HasColumnName("is_admin");

        // individuals
        modelBuilder.Entity<IndividualProfile>().ToTable("individuals");
        modelBuilder.Entity<IndividualProfile>().HasKey(i => i.Id);
        modelBuilder.Entity<IndividualProfile>().Property(i => i.Id).HasColumnName("individual_id").ValueGeneratedOnAdd();
        modelBuilder.Entity<IndividualProfile>().Property(i => i.UserId).HasColumnName("user_id");
        modelBuilder.Entity<IndividualProfile>().Property(i => i.FirstName).HasColumnName("first_name");
        modelBuilder.Entity<IndividualProfile>().Property(i => i.LastName).HasColumnName("last_name");
        modelBuilder.Entity<IndividualProfile>().Property(i => i.Phone).HasColumnName("phone");
        modelBuilder.Entity<IndividualProfile>().Property(i => i.City).HasColumnName("city");
        modelBuilder.Entity<IndividualProfile>().Property(i => i.District).HasColumnName("district");
        modelBuilder.Entity<IndividualProfile>().Property(i => i.AdditionalInfo).HasColumnName("additional_info");
        modelBuilder.Entity<IndividualProfile>().Property(i => i.PhotoUrl).HasColumnName("photo_url");

        // shelters
        modelBuilder.Entity<ShelterProfile>().ToTable("shelters");
        modelBuilder.Entity<ShelterProfile>().HasKey(s => s.Id);
        modelBuilder.Entity<ShelterProfile>().Property(s => s.Id).HasColumnName("shelter_id").ValueGeneratedOnAdd();
        modelBuilder.Entity<ShelterProfile>().Property(s => s.UserId).HasColumnName("user_id");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.Name).HasColumnName("name");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.ContactPerson).HasColumnName("contact_person");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.Phone).HasColumnName("phone");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.AdditionalPhone).HasColumnName("additional_phone");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.Address).HasColumnName("address");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.Description).HasColumnName("description");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.FacebookUrl).HasColumnName("facebook_url");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.InstagramUrl).HasColumnName("instagram_url");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.WebsiteUrl).HasColumnName("website_url");
        modelBuilder.Entity<ShelterProfile>().Property(s => s.LogoUrl).HasColumnName("logo_url");

        // listings
        modelBuilder.Entity<Listing>().ToTable("listings");
        modelBuilder.Entity<Listing>().HasKey(l => l.Id);
        modelBuilder.Entity<Listing>().Property(l => l.Id).HasColumnName("listing_id").ValueGeneratedOnAdd();
        modelBuilder.Entity<Listing>().Property(l => l.UserId).HasColumnName("user_id");
        modelBuilder.Entity<Listing>().Property(l => l.AnimalType).HasColumnName("animal_type");
        modelBuilder.Entity<Listing>().Property(l => l.Breed).HasColumnName("breed");
        modelBuilder.Entity<Listing>().Property(l => l.AgeMonths).HasColumnName("age_months");
        modelBuilder.Entity<Listing>().Property(l => l.Sex).HasColumnName("sex");
        modelBuilder.Entity<Listing>().Property(l => l.Size).HasColumnName("size");
        modelBuilder.Entity<Listing>().Property(l => l.Color).HasColumnName("color");
        modelBuilder.Entity<Listing>().Property(l => l.City).HasColumnName("city");
        modelBuilder.Entity<Listing>().Property(l => l.District).HasColumnName("district");
        modelBuilder.Entity<Listing>().Property(l => l.Description).HasColumnName("description");
        modelBuilder.Entity<Listing>().Property(l => l.SpecialNeeds).HasColumnName("special_needs");
        modelBuilder.Entity<Listing>().Property(l => l.Status).HasColumnName("status");
        modelBuilder.Entity<Listing>().Property(l => l.ViewsCount).HasColumnName("views_count");
        modelBuilder.Entity<Listing>().Property(l => l.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Listing>().Property(l => l.UpdatedAt).HasColumnName("updated_at");
        modelBuilder.Entity<Listing>().Property(l => l.ModerationComment).HasColumnName("moderation_comment");

        // photos
        modelBuilder.Entity<ListingPhoto>().ToTable("photos");
        modelBuilder.Entity<ListingPhoto>().HasKey(p => p.Id);
        modelBuilder.Entity<ListingPhoto>().Property(p => p.Id).HasColumnName("photo_id");
        modelBuilder.Entity<ListingPhoto>().Property(p => p.ListingId).HasColumnName("listing_id");
        modelBuilder.Entity<ListingPhoto>().Property(p => p.Url).HasColumnName("url");
        modelBuilder.Entity<ListingPhoto>().Property(p => p.IsPrimary).HasColumnName("is_primary");

        // health_info
        modelBuilder.Entity<HealthInfo>().ToTable("health_info");
        modelBuilder.Entity<HealthInfo>().HasKey(h => h.Id);
        modelBuilder.Entity<HealthInfo>().Property(h => h.Id).HasColumnName("health_id");
        modelBuilder.Entity<HealthInfo>().Property(h => h.ListingId).HasColumnName("listing_id");
        modelBuilder.Entity<HealthInfo>().Property(h => h.Vaccinations).HasColumnName("vaccinations");
        modelBuilder.Entity<HealthInfo>().Property(h => h.Sterilized).HasColumnName("sterilized");
        modelBuilder.Entity<HealthInfo>().Property(h => h.ChronicDiseases).HasColumnName("chronic_diseases");
        modelBuilder.Entity<HealthInfo>().Property(h => h.TreatmentHistory).HasColumnName("treatment_history");

        // favorites
        modelBuilder.Entity<Favorite>().ToTable("favorites");
        modelBuilder.Entity<Favorite>().HasKey(f => f.Id);
        modelBuilder.Entity<Favorite>().Property(f => f.Id).HasColumnName("favorite_id");
        modelBuilder.Entity<Favorite>().Property(f => f.UserId).HasColumnName("user_id");
        modelBuilder.Entity<Favorite>().Property(f => f.ListingId).HasColumnName("listing_id");
        modelBuilder.Entity<Favorite>().Property(f => f.CreatedAt).HasColumnName("created_at");

        // conversations
        modelBuilder.Entity<Conversation>().ToTable("conversations");
        modelBuilder.Entity<Conversation>().HasKey(c => c.Id);
        modelBuilder.Entity<Conversation>().Property(c => c.Id).HasColumnName("conversation_id");
        modelBuilder.Entity<Conversation>().Property(c => c.User1Id).HasColumnName("user1_id");
        modelBuilder.Entity<Conversation>().Property(c => c.User2Id).HasColumnName("user2_id");
        modelBuilder.Entity<Conversation>().Property(c => c.ListingId).HasColumnName("listing_id");
        modelBuilder.Entity<Conversation>().Property(c => c.LastMessageAt).HasColumnName("last_message_at");

        // messages
        modelBuilder.Entity<Message>().ToTable("messages");
        modelBuilder.Entity<Message>().HasKey(m => m.Id);
        modelBuilder.Entity<Message>().Property(m => m.Id).HasColumnName("message_id");
        modelBuilder.Entity<Message>().Property(m => m.ConversationId).HasColumnName("conversation_id");
        modelBuilder.Entity<Message>().Property(m => m.SenderId).HasColumnName("sender_id");
        modelBuilder.Entity<Message>().Property(m => m.Content).HasColumnName("content");
        modelBuilder.Entity<Message>().Property(m => m.IsRead).HasColumnName("is_read");
        modelBuilder.Entity<Message>().Property(m => m.CreatedAt).HasColumnName("created_at");

        // reviews
        modelBuilder.Entity<Review>().ToTable("reviews");
        modelBuilder.Entity<Review>().HasKey(r => r.Id);
        modelBuilder.Entity<Review>().Property(r => r.Id).HasColumnName("review_id");
        modelBuilder.Entity<Review>().Property(r => r.ReviewerId).HasColumnName("reviewer_id");
        modelBuilder.Entity<Review>().Property(r => r.ReviewedId).HasColumnName("reviewed_id");
        modelBuilder.Entity<Review>().Property(r => r.Rating).HasColumnName("rating");
        modelBuilder.Entity<Review>().Property(r => r.Comment).HasColumnName("comment");
        modelBuilder.Entity<Review>().Property(r => r.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Review>().Property(r => r.IsModerated).HasColumnName("is_moderated");

        // reports
        modelBuilder.Entity<Report>().ToTable("reports");
        modelBuilder.Entity<Report>().HasKey(r => r.Id);
        modelBuilder.Entity<Report>().Property(r => r.Id).HasColumnName("report_id");
        modelBuilder.Entity<Report>().Property(r => r.ReporterId).HasColumnName("reporter_id");
        modelBuilder.Entity<Report>().Property(r => r.ReportedType).HasColumnName("reported_type");
        modelBuilder.Entity<Report>().Property(r => r.ReportedEntityId).HasColumnName("reported_id");
        modelBuilder.Entity<Report>().Property(r => r.Reason).HasColumnName("reason");
        modelBuilder.Entity<Report>().Property(r => r.Description).HasColumnName("description");
        modelBuilder.Entity<Report>().Property(r => r.Status).HasColumnName("status");
        modelBuilder.Entity<Report>().Property(r => r.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Report>().Property(r => r.ResolvedAt).HasColumnName("resolved_at");

        // sessions
        modelBuilder.Entity<Session>().ToTable("sessions");
        modelBuilder.Entity<Session>().HasKey(s => s.Id);
        modelBuilder.Entity<Session>().Property(s => s.Id).HasColumnName("session_id");
        modelBuilder.Entity<Session>().Property(s => s.UserId).HasColumnName("user_id");
        modelBuilder.Entity<Session>().Property(s => s.SessionToken).HasColumnName("session_token");
        modelBuilder.Entity<Session>().Property(s => s.CreatedAt).HasColumnName("created_at");
        modelBuilder.Entity<Session>().Property(s => s.ExpiresAt).HasColumnName("expires_at");
        modelBuilder.Entity<Session>().Property(s => s.LastActivity).HasColumnName("last_activity");
        modelBuilder.Entity<Session>().Property(s => s.IsValid).HasColumnName("is_valid");

        //1-1: RegisteredUser <-> IndividualProfile
        modelBuilder.Entity<IndividualProfile>()
            .HasOne(i => i.User)
            .WithOne(u => u.IndividualProfile)
            .HasForeignKey<IndividualProfile>(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<IndividualProfile>()
            .HasIndex(i => i.UserId)
            .IsUnique();

        //1-1: RegisteredUser <-> ShelterProfile
        modelBuilder.Entity<ShelterProfile>()
            .HasOne(s => s.User)
            .WithOne(u => u.ShelterProfile)
            .HasForeignKey<ShelterProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ShelterProfile>()
            .HasIndex(s => s.UserId)
            .IsUnique();
    }
}