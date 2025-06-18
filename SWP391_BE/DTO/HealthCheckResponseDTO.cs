using System;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class HealthCheckResponseDTO
    {
        public int HealthCheckID { get; set; }
        public int StudentID { get; set; }
        public int NurseID { get; set; }
        public int ParentID { get; set; }
        public string Result { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public bool? ConfirmByParent { get; set; }
    }
}