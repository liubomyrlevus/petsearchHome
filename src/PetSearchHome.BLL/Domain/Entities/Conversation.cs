using System;
using System.Collections.Generic;
namespace PetSearchHome.BLL.Domain.Entities;

public class Conversation
{
    public int Id { get; set; }

    public int User1Id { get; set; }

    public int User2Id { get; set; }

    public int? ListingId { get; set; }

    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

 
    public List<Message> Messages { get; set; } = new List<Message>();
}
