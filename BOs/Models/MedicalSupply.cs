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
        public ICollection<MedicalEvent> MedicalEvents { get; set; }
    }
}