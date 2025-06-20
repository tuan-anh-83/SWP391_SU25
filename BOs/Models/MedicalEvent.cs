using System;
using System.Collections.Generic;

namespace BOs.Models
{
    public class MedicalEvent
    {
        public int MedicalEventId { get; set; }
        public int StudentId { get; set; }
        public int NurseId { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }

        public Student Student { get; set; }
        public Account Nurse { get; set; }

        // Navigation for many-to-many with payload
        public ICollection<MedicalEventMedication> MedicalEventMedications { get; set; }
        public ICollection<MedicalEventMedicalSupply> MedicalEventMedicalSupplies { get; set; }
    }
}