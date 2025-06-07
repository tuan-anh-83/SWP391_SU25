using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class HealthRecord
    {
        public int HealthRecordId { get; set; }
        public int ParentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Note { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double BMI { get; set; }
        public string NutritionStatus { get; set; }
        public Student Student { get; set; }
        public Account Parent { get; set; }
    }
    public enum NutritionStatus
    {
        Underweight = 0,
        Normal = 1,
        Overweight = 2,
        Obese = 3,
        ExtremlyObese =4,
    }

}
