using Microsoft.EntityFrameworkCore;
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

        modelBuilder.Entity<RegisteredUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd(); 
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasOne(u => u.IndividualProfile).WithOne(i => i.User).HasForeignKey<IndividualProfile>(i => i.UserId);
            entity.HasOne(u => u.ShelterProfile).WithOne(s => s.User).HasForeignKey<ShelterProfile>(s => s.UserId);
        });

        modelBuilder.Entity<IndividualProfile>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<ShelterProfile>().Property(e => e.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Listing>(entity => {
            entity.HasOne(l => l.HealthInfo).WithOne(h => h.Listing).HasForeignKey<HealthInfo>(h => h.ListingId);
            entity.HasMany(l => l.Photos).WithOne(p => p.Listing).HasForeignKey(p => p.ListingId);
        });

        modelBuilder.Entity<ListingPhoto>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<HealthInfo>().Property(e => e.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Session>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Favorite>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Conversation>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Message>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Review>().Property(e => e.Id).ValueGeneratedOnAdd();
        modelBuilder.Entity<Report>().Property(e => e.Id).ValueGeneratedOnAdd();
    }
}