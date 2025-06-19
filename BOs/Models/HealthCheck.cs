using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class HealthCheck
    {
        public int HealthCheckID { get; set; }
        public int StudentID { get; set; }
        public int NurseID { get; set; }
        public int ParentID { get; set; }
        public string? Result { get; set; }
        public DateTime Date { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public double? BMI { get; set; }
        public string? NutritionStatus { get; set; }

        public Student Student { get; set; }
        public Account Parent { get; set; }
        public Account Nurse { get; set; }
    }
}
