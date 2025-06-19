using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class CreateHealthCheckDTO
    {
        public int NurseId { get; set; }
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string HealthCheckDescription { get; set; }
    }
} 