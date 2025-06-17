using System;

namespace SWP391_BE.DTO
{
    public class HealthCheckResponseDTO
    {
        public int HealthCheckID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public int NurseID { get; set; }
        public string NurseName { get; set; }
        public int ParentID { get; set; }
        public string ParentName { get; set; }
        public string Result { get; set; }
        public DateTime Date { get; set; }
        public bool? ConfirmByParent { get; set; }
    }
}