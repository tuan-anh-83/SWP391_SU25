using System;

namespace SWP391_BE.DTO
{
    public class VaccinationFollowUpCreateDTO
    {
        public int RecordId { get; set; }
        public DateTime Date { get; set; }
        public string? Reaction { get; set; }
        public string? Note { get; set; }
    }
}
