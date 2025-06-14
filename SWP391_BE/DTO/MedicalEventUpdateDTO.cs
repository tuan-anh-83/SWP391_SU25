using System;
using System.Collections.Generic;

public class MedicalEventUpdateDTO
{
    public string? Type { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public DateTime? Date { get; set; }
    public List<int>? MedicationIds { get; set; } // Có thể null
    public List<int>? MedicalSupplyIds { get; set; } // Có thể null
}