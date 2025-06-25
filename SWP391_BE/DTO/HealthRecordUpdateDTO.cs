namespace SWP391_BE.DTO
{
    public class HealthRecordUpdateDTO
    {
        public double Height { get; set; }
        public double Weight { get; set; }

        public double LeftEye { get; set; }
        public double RightEye { get; set; }
        public string? Note { get; set; }
    }
}
