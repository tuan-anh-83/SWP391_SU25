using System;
using System.Collections.Generic;

namespace BOs.Models
{
    public class MedicalSupply
    {
        public int MedicalSupplyId { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public int Quantity { get; set; }

        // Navigation for many-to-many with payload
        public ICollection<MedicalEventMedicalSupply> MedicalEventMedicalSupplies { get; set; }
    }
}