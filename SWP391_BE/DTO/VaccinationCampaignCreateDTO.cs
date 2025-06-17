namespace SWP391_BE.DTO
{
    public class VaccinationCampaignCreateDTO
    {
        public string Name { get; set; }
        public int VaccineId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public List<int> ClassIds { get; set; } // Cho phép chọn nhiều lớp
    }
}
