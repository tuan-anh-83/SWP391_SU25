using System;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class HealthCheckListResponseDTO
    {
        public int HealthCheckID { get; set; }
        public string StudentName { get; set; }
        public string NurseName { get; set; }
        public string Result { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public bool? ConfirmByParent { get; set; }
    }
}