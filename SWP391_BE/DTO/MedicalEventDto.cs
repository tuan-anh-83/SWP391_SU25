namespace SWP391_BE.DTO
{
    public class MedicalEventDto
    {
        public int MedicalEventId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int NurseId { get; set; }
        public string NurseName { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public List<MedicationDto>? Medications { get; set; }
        public List<MedicalSupplyDto>? MedicalSupplies { get; set; }
    }

    public class MedicationDto
    {
        public int MedicationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Usage { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    public class MedicalSupplyDto
    {
        public int MedicalSupplyId { get; set; }
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}