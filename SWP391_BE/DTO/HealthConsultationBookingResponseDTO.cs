using BOs.Models;

namespace SWP391_BE.DTO
{
    public class HealthConsultationBookingResponseDTO
    {
        public int BookingId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public int NurseId { get; set; }
        public string NurseName { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; }

        public HealthConsultationBookingResponseDTO(HealthConsultationBooking b)
        {
            BookingId = b.BookingId;
            StudentId = b.StudentId;
            StudentName = b.Student?.Fullname;
            ClassName = b.Student?.Class?.ClassName;
            NurseId = b.NurseId;
            NurseName = b.Nurse?.Fullname;
            ParentId = b.ParentId;
            ParentName = b.Parent?.Fullname;
            ScheduledTime = b.ScheduledTime;
            Reason = b.Reason;
            Status = b.Status;
        }
    }
}
