using System;

namespace SWP391_BE.DTO
{
    public class ParentMedicationDetailDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Usage { get; set; }
        public string Dosage { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string Note { get; set; }
    }
}
