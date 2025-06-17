using System;
using System.Collections.Generic;

namespace BOs.Models
{
    public class VaccinationCampaign
    {
        public int CampaignId { get; set; }
        public string Name { get; set; }
        public int VaccineId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }

        public Vaccine Vaccine { get; set; }
        public ICollection<VaccinationConsent> Consents { get; set; }
        public ICollection<VaccinationRecord> Records { get; set; }
    }
}