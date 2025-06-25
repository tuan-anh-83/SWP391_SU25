using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class HealthRecordService : IHealthRecordService
    {
        private readonly IHealthRecordRepo _healthRecordRepo;
        private readonly IStudentRepo _studentRepo;

        public HealthRecordService(IHealthRecordRepo healthRecordRepo, IStudentRepo studentRepo)
        {
            _healthRecordRepo = healthRecordRepo;
            _studentRepo = studentRepo;
        }

        public async Task<HealthRecord> CreateHealthRecordAsync(HealthRecord healthRecord)
        {
            // Lấy thông tin Student từ DB bằng StudentCode
            var student = await _studentRepo.GetStudentByCodeAsync(healthRecord.StudentCode);
            if (student == null)
                throw new Exception("Student not found");

            healthRecord.StudentId = student.StudentId;
            healthRecord.StudentName = student.Fullname;
            healthRecord.Gender = student.Gender;
            healthRecord.DateOfBirth = student.DateOfBirth;

            return await _healthRecordRepo.CreateHealthRecordAsync(healthRecord);
        }

        public async Task<HealthRecord?> GetHealthRecordByIdAsync(int id)
        {
            return await _healthRecordRepo.GetHealthRecordByIdAsync(id);
        }

        public async Task<List<HealthRecord>> GetAllHealthRecordsAsync()
        {
            return await _healthRecordRepo.GetAllHealthRecordsAsync();
        }

        public async Task<List<HealthRecord>> GetHealthRecordsByStudentIdAsync(int studentId)
        {
            return await _healthRecordRepo.GetHealthRecordsByStudentIdAsync(studentId);
        }

        public async Task<HealthRecord?> UpdateHealthRecordAsync(HealthRecord healthRecord)
        {
            var student = await _studentRepo.GetStudentByCodeAsync(healthRecord.StudentCode);
            if (student == null)
                throw new Exception("Student not found");

            healthRecord.StudentId = student.StudentId;
            healthRecord.StudentName = student.Fullname;
            healthRecord.Gender = student.Gender;
            healthRecord.DateOfBirth = student.DateOfBirth;

            return await _healthRecordRepo.UpdateHealthRecordAsync(healthRecord);
        }

        public async Task<bool> DeleteHealthRecordAsync(int id)
        {
            return await _healthRecordRepo.DeleteHealthRecordAsync(id);
        }

        // private double CalculateBMI(double height, double weight)
        // {
        //     if (height <= 0 || weight <= 0)
        //         throw new ArgumentException("Height and weight must be greater than 0");
        //     double heightInMeters = height / 100;
        //     double bmi = weight / (heightInMeters * heightInMeters);
        //     return Math.Round(bmi, 2);
        // }

        // private string DetermineNutritionStatus(double bmi)
        // {
        //     if (bmi < 18.5) return "Underweight";
        //     if (bmi < 25) return "Normal";
        //     if (bmi < 30) return "Overweight";
        //     if (bmi < 40) return "Obese";
        //     return "ExtremlyObese";
        // }
    }
}