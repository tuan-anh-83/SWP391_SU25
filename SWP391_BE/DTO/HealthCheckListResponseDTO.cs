using System;

namespace SWP391_BE.DTO
{
    public class HealthCheckListResponseDTO
    {
        public int HealthCheckID { get; set; }
        public string StudentName { get; set; }
        public string NurseName { get; set; }
        public string Result { get; set; }
        public DateTime Date { get; set; }
        public bool? ConfirmByParent { get; set; }
    }
}