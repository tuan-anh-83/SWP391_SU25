namespace SWP391_BE.DTO
{
    public class HealthConsultationBookingCreateDTO
    {
        public string? StudentCode { get; set; } // Thêm dòng này
        public int? StudentId { get; set; }      // Để nullable, chỉ dùng cho Nurse/Admin
        public int NurseId { get; set; }
        // public int ParentId { get; set; }     // Bỏ dòng này, không cần truyền từ FE
        public DateTime ScheduledTime { get; set; }
        public string? Reason { get; set; }
    }
}
