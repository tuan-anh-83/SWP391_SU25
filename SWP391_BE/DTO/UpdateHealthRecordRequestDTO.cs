using System;

namespace SWP391_BE.DTO
{
    public class UpdateHealthRecordRequestDTO
    {
        public int HealthRecordId { get; set; }
        public string Note { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }
} 