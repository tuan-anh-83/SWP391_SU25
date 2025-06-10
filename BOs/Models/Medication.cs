namespace BOs.Models
{
    public class Medication
    {
        public int MedicationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Usage { get; set; }
        public DateTime ExpiredDate { get; set; }
        public ICollection<MedicalEvent> MedicalEvents { get; set; }
    }
}