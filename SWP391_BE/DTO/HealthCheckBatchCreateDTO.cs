using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class HealthCheckBatchCreateDTO
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public int NurseId { get; set; }
        public List<int> ClassIds { get; set; }
        public string HealthCheckDescription { get; set; }
    }
} 