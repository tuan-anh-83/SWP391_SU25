using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO;

public class MedicalEventCreateDTO
{
    [Required]
    public int StudentId { get; set; }
    [Required]
    public string Type { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    [Required]
    public DateTime Date { get; set; }
    public List<MedicalEventMedicationDTO>? Medications { get; set; }
    public List<MedicalEventSupplyDTO>? MedicalSupplies { get; set; }
}

public class MedicalEventMedicationDTO
{
    public int MedicationId { get; set; }
    public int QuantityUsed { get; set; }
}

public class MedicalEventSupplyDTO
{
    public int MedicalSupplyId { get; set; }
    public int QuantityUsed { get; set; }
}

