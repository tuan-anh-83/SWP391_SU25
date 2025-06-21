namespace SWP391_BE.DTO
{
    public class HealthConsultationBookingCreateDTO
    {
        public int StudentId { get; set; }
        public int NurseId { get; set; }
        public int ParentId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string? Reason { get; set; }
    }
}
