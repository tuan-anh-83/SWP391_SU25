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
    public List<int>? MedicationIds { get; set; } // Có thể null
    public List<int>? MedicalSupplyIds { get; set; } // Có thể null
}

