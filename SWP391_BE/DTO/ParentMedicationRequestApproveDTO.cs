namespace SWP391_BE.DTO
{
    public class ParentMedicationRequestApproveDTO
    {
        public string Status { get; set; } // "Approved" hoặc "Rejected"
        public string? NurseNote { get; set; }
    }
}
