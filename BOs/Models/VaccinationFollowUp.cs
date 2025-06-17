using System;

namespace BOs.Models
{
    public class VaccinationFollowUp
    {
        public int FollowUpId { get; set; }
        public int RecordId { get; set; }
        public DateTime Date { get; set; }
        public string? Reaction { get; set; }
        public string? Note { get; set; }

        public VaccinationRecord Record { get; set; }
    }
}