namespace SWP391_BE.DTO
{
    public class VaccinationRecordCreateDTO
    {
        public int CampaignId { get; set; }
        public int StudentId { get; set; }
        public int NurseId { get; set; }
        public DateTime DateInjected { get; set; }
        public string Result { get; set; }
        public string? ImmediateReaction { get; set; }
        public string? Note { get; set; }
    }
}
