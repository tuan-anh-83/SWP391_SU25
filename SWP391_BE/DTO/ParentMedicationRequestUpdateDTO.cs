using System.Collections.Generic;

namespace SWP391_BE.DTO
{
    public class ParentMedicationRequestUpdateDTO
    {
        public string? ParentNote { get; set; }
        public List<ParentMedicationDetailDTO> Medications { get; set; }
    }
}