using SWP391_BE.DTO;
using System;
using System.Collections.Generic;

public class MedicalEventUpdateDTO
{
    public string? Type { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public DateTime? Date { get; set; }
    public List<MedicalEventMedicationDTO>? Medications { get; set; }
    public List<MedicalEventSupplyDTO>? MedicalSupplies { get; set; }
}