namespace PetSearchHome.BLL.Domain.Entities;
public class ListingPhoto
{
    public int Id { get; set; } 
    public int ListingId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }

    public Listing Listing { get; set; } = null!;
}