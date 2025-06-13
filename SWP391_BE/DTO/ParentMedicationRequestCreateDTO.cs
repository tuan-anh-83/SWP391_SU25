using System.Collections.Generic;

namespace SWP391_BE.DTO
{
    public class ParentMedicationRequestCreateDTO
    {
        public int StudentId { get; set; }
        public string? ParentNote { get; set; } // Cho phép null
        public List<ParentMedicationDetailDTO> Medications { get; set; }
    }
}
