namespace SWP391_BE.DTO
{
    public class VaccinationConsentCreateDTO
    {
        public int CampaignId { get; set; }
        public int StudentId { get; set; }
        public bool? IsAgreed { get; set; }
        public string? Note { get; set; }
    }
}
