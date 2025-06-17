using System;
using System.Collections.Generic;

namespace BOs.Models
{
    public class Vaccine
    {
        public int VaccineId { get; set; }
        public string Name { get; set; } // Ví dụ: "TeenVaccine"
        public string Description { get; set; } // Ví dụ: "Tiêm bệnh sởi"
        public ICollection<VaccinationCampaign> Campaigns { get; set; }
    }
}