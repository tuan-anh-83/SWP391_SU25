using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class UpdateHealthCheckRequestDTO
    {
        public int HealthCheckID { get; set; }
        public string Result { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}