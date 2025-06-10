namespace SWP391_BE.DTO
{
    public class HealthRecordCreateDTO
    {
        public int ParentId { get; set; }      
        public string StudentCode { get; set; } 
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Note { get; set; }
    }
}
