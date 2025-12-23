namespace PetSearchHome.BLL.Domain.Entities;
public class HealthInfo
{
    public int Id { get; set; } 
    public int ListingId { get; set; }
    public string? Vaccinations { get; set; }
    public bool? Sterilized { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? TreatmentHistory { get; set; }

    public Listing Listing { get; set; } = null!;
}