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

        public HealthRecordService(IHealthRecordRepo healthRecordRepo)
        {
            _healthRecordRepo = healthRecordRepo;
        }

        public async Task<HealthRecord> CreateHealthRecord(HealthRecord healthRecord)
        {
            try
            {
                // Calculate BMI and determine nutrition status before saving
                healthRecord.BMI = await CalculateBMI(healthRecord.Height, healthRecord.Weight);
                healthRecord.NutritionStatus = await DetermineNutritionStatus(healthRecord.BMI);
                
                return await _healthRecordRepo.CreateHealthRecord(healthRecord);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in service layer while creating health record: " + ex.Message);
            }
        }

        public async Task<HealthRecord> GetHealthRecordById(int id)
        {
            try
            {
                var healthRecord = await _healthRecordRepo.GetHealthRecordById(id);
                if (healthRecord == null)
                {
                    throw new Exception("Health record not found");
                }
                return healthRecord;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in service layer while retrieving health record: " + ex.Message);
            }
        }

        public async Task<List<HealthRecord>> GetAllHealthRecords()
        {
            try
            {
                return await _healthRecordRepo.GetAllHealthRecords();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in service layer while retrieving all health records: " + ex.Message);
            }
        }

        public async Task<List<HealthRecord>> GetHealthRecordsByStudentId(int studentId)
        {
            try
            {
                return await _healthRecordRepo.GetHealthRecordsByStudentId(studentId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in service layer while retrieving health records by student ID: " + ex.Message);
            }
        }

        public async Task<HealthRecord> UpdateHealthRecord(HealthRecord healthRecord)
        {
            try
            {
                // Recalculate BMI and nutrition status if height or weight changed
                healthRecord.BMI = await CalculateBMI(healthRecord.Height, healthRecord.Weight);
                healthRecord.NutritionStatus = await DetermineNutritionStatus(healthRecord.BMI);
                
                return await _healthRecordRepo.UpdateHealthRecord(healthRecord);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in service layer while updating health record: " + ex.Message);
            }
        }

        public async Task<bool> DeleteHealthRecord(int id)
        {
            try
            {
                return await _healthRecordRepo.DeleteHealthRecord(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in service layer while deleting health record: " + ex.Message);
            }
        }

        public async Task<double> CalculateBMI(double height, double weight)
        {
            try
            {
                // Height should be in meters, weight in kilograms
                if (height <= 0 || weight <= 0)
                {
                    throw new ArgumentException("Height and weight must be greater than 0");
                }

                // Convert height from cm to meters if needed
                double heightInMeters = height > 3 ? height / 100 : height;
                
                // Calculate BMI: weight (kg) / (height (m) * height (m))
                double bmi = weight / (heightInMeters * heightInMeters);
                return Math.Round(bmi, 2);
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating BMI: " + ex.Message);
            }
        }

        public async Task<string> DetermineNutritionStatus(double bmi)
        {
            try
            {
                if (bmi < 18.5)
                    return NutritionStatus.Underweight.ToString();
                else if (bmi < 25)
                    return NutritionStatus.Normal.ToString();
                else if (bmi < 30)
                    return NutritionStatus.Overweight.ToString();
                else if (bmi < 40)
                    return NutritionStatus.Obese.ToString();
                else
                    return NutritionStatus.ExtremlyObese.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error determining nutrition status: " + ex.Message);
            }
        }
    }
} 