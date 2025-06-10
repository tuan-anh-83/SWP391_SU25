using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO;

public class MedicalEventCreateDTO
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public string Type { get; set; }

    public string Description { get; set; }

    public string Note { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public int? MedicationId { get; set; }
}

public class MedicalEventUpdateModel
{
    public string Type { get; set; }
    public string Description { get; set; }
    public string Note { get; set; }
    public DateTime? Date { get; set; }
    public int? MedicationId { get; set; }
}