using System;
using System.ComponentModel.DataAnnotations;
using BOs.Models;

namespace SWP391_BE.DTO
{
    public class HealthCheckResponseDTO
    {
        public int HealthCheckID { get; set; }
        public int StudentID { get; set; }
        public int NurseID { get; set; }
        public int? ParentID { get; set; }
        public string? Result { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        //public double? BMI { get; set; }
        //public string? NutritionStatus { get; set; }
        public double? LeftEye { get; set; }
        public double? RightEye { get; set; }
        public string? HealthCheckDescription { get; set; }

        public HealthCheckResponseDTO(HealthCheck hc)
        {
            HealthCheckID = hc.HealthCheckID;
            StudentID = hc.StudentID;
            NurseID = hc.NurseID;
            ParentID = hc.ParentID;
            Result = hc.Result;
            Date = hc.Date;
            Height = hc.Height;
            Weight = hc.Weight;
            //BMI = hc.BMI;
            //NutritionStatus = hc.NutritionStatus;
            LeftEye = hc.LeftEye;
            RightEye = hc.RightEye;
            HealthCheckDescription = hc.HealthCheckDescription;
        }
    }
} 