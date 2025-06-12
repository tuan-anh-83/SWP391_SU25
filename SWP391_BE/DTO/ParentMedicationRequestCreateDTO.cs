namespace SWP391_BE.DTO
{
    public class ParentMedicationRequestCreateDTO
    {
        public int StudentId { get; set; }
        public string ParentNote { get; set; }
        public List<int> MedicationIds { get; set; }
    }
}
