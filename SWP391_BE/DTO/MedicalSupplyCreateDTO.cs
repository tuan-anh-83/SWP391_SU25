using System;

namespace SWP391_BE.DTO
{
    public class MedicalSupplyCreateDTO
    {
        public string Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}
