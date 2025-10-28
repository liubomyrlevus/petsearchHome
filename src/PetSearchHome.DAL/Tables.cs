using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSearchHome.DAL;

public enum UserType
{
    individual,
    shelter
}

public enum AnimalType
{
    dog,
    cat,
    bird,
    other
}

public enum AnimalSex
{
    male,
    female,
    unknown
}

public enum AnimalSize
{
    small,
    medium,
    large
}

public enum ListingStatus
{
    draft,
    pending,
    active,
    rejected,
    archived
}

public enum ReportType
{
    listing,
    user,
    message
}

public enum ReportStatus
{
    pending,
    confirmed,
    rejected
}


public class RegisteredUser
{
    [Key]
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }

    public Individual Individual { get; set; } = null!;

    public Shelter Shelter { get; set; } = null!;

    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();

    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();

    public ICollection<Report> ReportsSent { get; set; } = new List<Report>();

    public ICollection<Message> MessagesSent { get; set; } = new List<Message>();

    public ICollection<Conversation> ConversationsAsUser1 { get; set; } = new List<Conversation>();
    public ICollection<Conversation> ConversationsAsUser2 { get; set; } = new List<Conversation>();
}

public class Session
{
    [Key]
    public Guid SessionId { get; set; }
    public int UserId { get; set; }
    public string SessionToken { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsValid { get; set; }

    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; } = null!;
}

public class Individual
{
    [Key]
    public int IndividualId { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? PhotoUrl { get; set; }

    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; } = null!;
}

public class Shelter
{
    [Key]
    public int ShelterId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? AdditionalPhone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoUrl { get; set; }

    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; } = null!;
}

public class Listing
{
    [Key]
    public int ListingId { get; set; }
    public int UserId { get; set; }
    public AnimalType AnimalType { get; set; }
    public string? Breed { get; set; }
    public int? AgeMonths { get; set; }
    public AnimalSex? Sex { get; set; }
    public AnimalSize? Size { get; set; }
    public string? Color { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Description { get; set; }
    public string? SpecialNeeds { get; set; }
    public ListingStatus Status { get; set; }
    public int ViewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? ModerationComment { get; set; }

    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; } = null!;

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();

    public HealthInfo HealthInfo { get; set; } = null!;

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}

public class Photo
{
    [Key]
    public int PhotoId { get; set; }
    public int ListingId { get; set; }
    public string Url { get; set; } = null!;
    public bool IsPrimary { get; set; }

    [ForeignKey("ListingId")]
    public Listing Listing { get; set; } = null!;
}

public class HealthInfo
{
    [Key]
    public int HealthId { get; set; }
    public int ListingId { get; set; }
    public string? Vaccinations { get; set; }
    public bool? Sterilized { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? TreatmentHistory { get; set; }

    [ForeignKey("ListingId")]
    public Listing Listing { get; set; } = null!;
}

public class Favorite
{
    [Key]
    public int FavoriteId { get; set; }
    public int UserId { get; set; }
    public int ListingId { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; } = null!;

    [ForeignKey("ListingId")]
    public Listing Listing { get; set; } = null!;
}

public class Conversation
{
    [Key]
    public int ConversationId { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }
    public int? ListingId { get; set; }
    public DateTime LastMessageAt { get; set; }

    [ForeignKey("User1Id")]
    public RegisteredUser User1 { get; set; } = null!;

    [ForeignKey("User2Id")]
    public RegisteredUser User2 { get; set; } = null!;

    [ForeignKey("ListingId")]
    public Listing? Listing { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class Message
{
    [Key]
    public int MessageId { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey("ConversationId")]
    public Conversation Conversation { get; set; } = null!;

    [ForeignKey("SenderId")]
    public RegisteredUser Sender { get; set; } = null!;
}

public class Review
{
    [Key]
    public int ReviewId { get; set; }
    public int ReviewerId { get; set; }
    public int ReviewedId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsModerated { get; set; }

    [ForeignKey("ReviewerId")]
    public RegisteredUser Reviewer { get; set; } = null!;

    [ForeignKey("ReviewedId")]
    public RegisteredUser Reviewed { get; set; } = null!;
}


public class Report
{
    [Key]
    public int ReportId { get; set; }
    public int ReporterId { get; set; }
    public ReportType ReportedType { get; set; }
    public int ReportedId { get; set; }
    public string? Reason { get; set; }
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    [ForeignKey("ReporterId")]
    public RegisteredUser Reporter { get; set; } = null!;
}

