using System;

namespace BOs.Models
{
    public class VaccinationConsent
    {
        public int ConsentId { get; set; }
        public int CampaignId { get; set; }
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public bool? IsAgreed { get; set; }
        public string? Note { get; set; }
        public DateTime? DateConfirmed { get; set; }

        public VaccinationCampaign Campaign { get; set; }
        public Student Student { get; set; }
        public Account Parent { get; set; }
    }
}