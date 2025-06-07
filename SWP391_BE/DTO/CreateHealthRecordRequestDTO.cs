using System;

namespace SWP391_BE.DTO
{
    public class CreateHealthRecordRequestDTO
    {
        public int ParentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Note { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }
} 