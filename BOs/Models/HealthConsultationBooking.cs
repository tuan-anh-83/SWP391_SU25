using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class HealthConsultationBooking
    {
        public int BookingId { get; set; }
        public int StudentId { get; set; }
        public int NurseId { get; set; }
        public int ParentId { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Cancelled, Done

        public Student Student { get; set; }
        public Account Nurse { get; set; }
        public Account Parent { get; set; }
    }
}
