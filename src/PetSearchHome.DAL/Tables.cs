//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace PetSearchHome.DAL; // Assuming DAL is the root namespace for these entities

//// Enums should be in a separate file, but are included here for completeness
//// Make sure these enums match the ones in your BLL project
//public enum UserType { Unknown, Individual, Shelter }
//public enum AnimalType { Unknown, Dog, Cat, Bird, Rodent, Reptile, Other }
//public enum AnimalSex { Unknown, Male, Female }
//public enum AnimalSize { Unknown, Small, Medium, Large, Giant }
//public enum ListingStatus { Draft, Pending, Active, Rejected, Archived }
//public enum ReportTargetType { Listing, User, Message }
//public enum ReportStatus { Pending, Confirmed, Rejected, Archived }

//public class RegisteredUser
//{
//    [Key]
//    public Guid Id { get; set; }
//    public string Email { get; set; } = string.Empty;
//    public string PasswordHash { get; set; } = string.Empty;
//    public UserType UserType { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public DateTime? LastLogin { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsAdmin { get; set; }

//    public IndividualProfile? IndividualProfile { get; set; }
//    public ShelterProfile? ShelterProfile { get; set; }
//}

//public class Session
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid UserId { get; set; }
//    public string SessionToken { get; set; } = string.Empty;
//    public DateTime CreatedAt { get; set; }
//    public DateTime ExpiresAt { get; set; }
//    public DateTime LastActivity { get; set; }
//    public bool IsValid { get; set; }

//    [ForeignKey("UserId")]
//    public RegisteredUser User { get; set; } = null!;
//}

//public class IndividualProfile
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid UserId { get; set; }
//    public string FirstName { get; set; } = string.Empty;
//    public string LastName { get; set; } = string.Empty;
//    public string Phone { get; set; } = string.Empty;
//    public string City { get; set; } = string.Empty;
//    public string District { get; set; } = string.Empty;
//    public string? AdditionalInfo { get; set; }
//    public string? PhotoUrl { get; set; }

//    [ForeignKey("UserId")]
//    public RegisteredUser User { get; set; } = null!;
//}

//public class ShelterProfile
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid UserId { get; set; }
//    public string Name { get; set; } = string.Empty;
//    public string ContactPerson { get; set; } = string.Empty;
//    public string Phone { get; set; } = string.Empty;
//    public string? AdditionalPhone { get; set; }
//    public string Address { get; set; } = string.Empty;
//    public string? Description { get; set; }
//    public string? FacebookUrl { get; set; }
//    public string? InstagramUrl { get; set; }
//    public string? WebsiteUrl { get; set; }
//    public string? LogoUrl { get; set; }

//    [ForeignKey("UserId")]
//    public RegisteredUser User { get; set; } = null!;
//}

//public class Listing
//{
//    [Key]
//    public int Id { get; set; }
//    public Guid UserId { get; set; }
//    public AnimalType AnimalType { get; set; }
//    public string Breed { get; set; } = string.Empty;
//    public int AgeMonths { get; set; }
//    public AnimalSex Sex { get; set; }
//    public AnimalSize Size { get; set; }
//    public string Color { get; set; } = string.Empty;
//    public string City { get; set; } = string.Empty;
//    public string District { get; set; } = string.Empty;
//    public string Description { get; set; } = string.Empty;
//    public string? SpecialNeeds { get; set; }
//    public ListingStatus Status { get; set; }
//    public int ViewsCount { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public DateTime? UpdatedAt { get; set; }
//    public string? ModerationComment { get; set; }

//    [ForeignKey("UserId")]
//    public RegisteredUser User { get; set; } = null!;
//    public HealthInfo? HealthInfo { get; set; }
//    public ICollection<ListingPhoto> Photos { get; set; } = new List<ListingPhoto>();
//}

//public class ListingPhoto
//{
//    [Key]
//    public Guid Id { get; set; }
//    public int ListingId { get; set; }
//    public string Url { get; set; } = string.Empty;
//    public bool IsPrimary { get; set; }

//    [ForeignKey("ListingId")]
//    public Listing Listing { get; set; } = null!;
//}

//public class HealthInfo
//{
//    [Key]
//    public Guid Id { get; set; }
//    public int ListingId { get; set; }
//    public string? Vaccinations { get; set; }
//    public bool? Sterilized { get; set; }
//    public string? ChronicDiseases { get; set; }
//    public string? TreatmentHistory { get; set; }

//    [ForeignKey("ListingId")]
//    public Listing Listing { get; set; } = null!;
//}

//public class Favorite
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid UserId { get; set; }
//    public int ListingId { get; set; }
//    public DateTime CreatedAt { get; set; }

//    [ForeignKey("UserId")]
//    public RegisteredUser User { get; set; } = null!;
//    [ForeignKey("ListingId")]
//    public Listing Listing { get; set; } = null!;
//}

//public class Conversation
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid User1Id { get; set; }
//    public Guid User2Id { get; set; }
//    public int? ListingId { get; set; }
//    public DateTime LastMessageAt { get; set; }

//    [ForeignKey("User1Id")]
//    public RegisteredUser User1 { get; set; } = null!;
//    [ForeignKey("User2Id")]
//    public RegisteredUser User2 { get; set; } = null!;
//    [ForeignKey("ListingId")]
//    public Listing? Listing { get; set; }
//    public ICollection<Message> Messages { get; set; } = new List<Message>();
//}

//public class Message
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid ConversationId { get; set; }
//    public Guid SenderId { get; set; }
//    public string Content { get; set; } = string.Empty;
//    public bool IsRead { get; set; }
//    public DateTime CreatedAt { get; set; }

//    [ForeignKey("ConversationId")]
//    public Conversation Conversation { get; set; } = null!;
//    [ForeignKey("SenderId")]
//    public RegisteredUser Sender { get; set; } = null!;
//}

//public class Review
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid ReviewerId { get; set; }
//    public Guid ReviewedId { get; set; }
//    public int Rating { get; set; }
//    public string? Comment { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public bool IsModerated { get; set; }

//    [ForeignKey("ReviewerId")]
//    public RegisteredUser Reviewer { get; set; } = null!;
//    [ForeignKey("ReviewedId")]
//    public RegisteredUser Reviewed { get; set; } = null!;
//}

//public class Report
//{
//    [Key]
//    public Guid Id { get; set; }
//    public Guid ReporterId { get; set; }
//    public ReportTargetType ReportedType { get; set; }
//    public Guid ReportedEntityId { get; set; }
//    public string Reason { get; set; } = string.Empty;
//    public string? Description { get; set; }
//    public ReportStatus Status { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public DateTime? ResolvedAt { get; set; }

//    [ForeignKey("ReporterId")]
//    public RegisteredUser Reporter { get; set; } = null!;
//}