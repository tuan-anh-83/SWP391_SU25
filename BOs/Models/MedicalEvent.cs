namespace BOs.Models
{
    public class MedicalEvent
    {
        public int MedicalEventId { get; set; }
        public int StudentId { get; set; }
        public int NurseId { get; set; }
        public int? MedicationId { get; set; } // Có thể null nếu không dùng thuốc
        public string Type { get; set; } // Loại sự kiện: tai nạn, sốt, té ngã, dịch bệnh,...
        public string Description { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }

        public Student Student { get; set; }
        public Account Nurse { get; set; }
        public Medication Medication { get; set; }
    }
}