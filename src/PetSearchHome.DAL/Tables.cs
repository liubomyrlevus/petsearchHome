using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSearchHome.DAL;

// Для registered_users.user_type
public enum UserType
{
    individual,
    shelter
}

// Для listings.animal_type
public enum AnimalType
{
    dog,
    cat,
    bird,
    other
}

// Для listings.sex
public enum AnimalSex
{
    male,
    female,
    unknown
}

// Для listings.size
public enum AnimalSize
{
    small,
    medium,
    large
}

// Для listings.status
public enum ListingStatus
{
    draft,
    pending,
    active,
    rejected,
    archived
}

// Для reports.reported_type
public enum ReportType
{
    listing,
    user,
    message
}

// Для reports.status
public enum ReportStatus
{
    pending,
    confirmed,
    rejected
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Таблиця: registered_users
public class RegisteredUser
{
    [Key]
    public int UserId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }

    // Навігаційні властивості (зв'язки)

    // 1-до-1 з Individual
    public Individual Individual { get; set; }

    // 1-до-1 з Shelter
    public Shelter Shelter { get; set; }

    // 1-до-багатьох з Sessions
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    // 1-до-багатьох з Listings
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();

    // 1-до-багатьох з Favorites
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    // 1-до-багатьох з Reviews (як рецензент)
    public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();

    // 1-до-багатьох з Reviews (як той, кого рецензують)
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();

    // 1-до-багатьох з Reports (як той, хто скаржиться)
    public ICollection<Report> ReportsSent { get; set; } = new List<Report>();

    // 1-до-багатьох з Messages (як відправник)
    public ICollection<Message> MessagesSent { get; set; } = new List<Message>();

    // Зв'язки для Conversations
    public ICollection<Conversation> ConversationsAsUser1 { get; set; } = new List<Conversation>();
    public ICollection<Conversation> ConversationsAsUser2 { get; set; } = new List<Conversation>();
}

// Таблиця: sessions
public class Session
{
    [Key]
    public Guid SessionId { get; set; }
    public int UserId { get; set; }
    public string SessionToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime LastActivity { get; set; }
    public bool IsValid { get; set; }

    // Зв'язок "багато-до-одного"
    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; }
}

// Таблиця: individuals
public class Individual
{
    [Key]
    public int IndividualId { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? PhotoUrl { get; set; }

    // Зв'язок "один-до-одного"
    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; }
}

// Таблиця: shelters
public class Shelter
{
    [Key]
    public int ShelterId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? AdditionalPhone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoUrl { get; set; }

    // Зв'язок "один-до-одного"
    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; }
}

// Таблиця: listings
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

    // Зв'язок "багато-до-одного"
    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; }

    // 1-до-багатьох з Photos
    public ICollection<Photo> Photos { get; set; } = new List<Photo>();

    // 1-до-1 з HealthInfo
    public HealthInfo HealthInfo { get; set; }

    // 1-до-багатьох з Favorites
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    // 1-до-багатьох з Conversations
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}

// Таблиця: photos
public class Photo
{
    [Key]
    public int PhotoId { get; set; }
    public int ListingId { get; set; }
    public string Url { get; set; }
    public bool IsPrimary { get; set; }

    // Зв'язок "багато-до-одного"
    [ForeignKey("ListingId")]
    public Listing Listing { get; set; }
}

// Таблиця: health_info
public class HealthInfo
{
    [Key]
    public int HealthId { get; set; }
    public int ListingId { get; set; }
    public string? Vaccinations { get; set; }
    public bool? Sterilized { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? TreatmentHistory { get; set; }

    // Зв'язок "один-до-одного"
    [ForeignKey("ListingId")]
    public Listing Listing { get; set; }
}

// Таблиця: favorites
public class Favorite
{
    [Key]
    public int FavoriteId { get; set; }
    public int UserId { get; set; }
    public int ListingId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Зв'язки "багато-до-одного"
    [ForeignKey("UserId")]
    public RegisteredUser User { get; set; }

    [ForeignKey("ListingId")]
    public Listing Listing { get; set; }
}

// Таблиця: conversations
public class Conversation
{
    [Key]
    public int ConversationId { get; set; }
    public int User1Id { get; set; }
    public int User2Id { get; set; }
    public int? ListingId { get; set; }
    public DateTime LastMessageAt { get; set; }

    // Зв'язки "багато-до-одного"
    [ForeignKey("User1Id")]
    public RegisteredUser User1 { get; set; }

    [ForeignKey("User2Id")]
    public RegisteredUser User2 { get; set; }

    [ForeignKey("ListingId")]
    public Listing? Listing { get; set; }

    // 1-до-багатьох з Messages
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

// Таблиця: messages
public class Message
{
    [Key]
    public int MessageId { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    // Зв'язки "багато-до-одного"
    [ForeignKey("ConversationId")]
    public Conversation Conversation { get; set; }

    [ForeignKey("SenderId")]
    public RegisteredUser Sender { get; set; }
}

// Таблиця: reviews
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

    // Зв'язки "багато-до-одного"
    [ForeignKey("ReviewerId")]
    public RegisteredUser Reviewer { get; set; }

    [ForeignKey("ReviewedId")]
    public RegisteredUser Reviewed { get; set; }
}

// Таблиця: reports
public class Report
{
    [Key]
    public int ReportId { get; set; }
    public int ReporterId { get; set; }
    public ReportType ReportedType { get; set; }
    public int ReportedId { get; set; } // ID оголошення, юзера або повідомлення
    public string? Reason { get; set; }
    public string? Description { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // Зв'язок "багато-до-одного"
    [ForeignKey("ReporterId")]
    public RegisteredUser Reporter { get; set; }
}
