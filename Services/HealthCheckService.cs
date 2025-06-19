using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepo _repo;

        public HealthCheckService()
        {
            _repo = new HealthCheckRepo();
        }

        public async Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck)
        {
            CalculateBmiAndNutritionStatus(healthCheck);
            return await _repo.CreateHealthCheckAsync(healthCheck);
        }

        public async Task<HealthCheck?> GetHealthCheckByIdAsync(int id)
            => await _repo.GetHealthCheckByIdAsync(id);

        public async Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId)
            => await _repo.GetHealthChecksByStudentIdAsync(studentId);

        public async Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId)
            => await _repo.GetHealthChecksByParentIdAsync(parentId);

        public async Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId)
            => await _repo.GetHealthChecksByNurseIdAsync(nurseId);

        public async Task<List<HealthCheck>> GetAllHealthChecksAsync()
            => await _repo.GetAllHealthChecksAsync();

        public async Task<HealthCheck?> UpdateHealthCheckAsync(HealthCheck healthCheck)
        {
            CalculateBmiAndNutritionStatus(healthCheck);
            return await _repo.UpdateHealthCheckAsync(healthCheck);
        }

        public async Task<bool> DeleteHealthCheckAsync(int id)
            => await _repo.DeleteHealthCheckAsync(id);

        public async Task<List<HealthCheck>> GetHealthChecksByDateAsync(DateTime date)
            => await _repo.GetHealthChecksByDateAsync(date);

        private void CalculateBmiAndNutritionStatus(HealthCheck healthCheck)
        {
            if (healthCheck.Height.HasValue && healthCheck.Weight.HasValue && healthCheck.Height.Value > 0)
            {
                // Height is expected in meters
                double calHeight=healthCheck.Height.Value/100;
                double bmi = healthCheck.Weight.Value / (calHeight * calHeight);
                healthCheck.BMI = Math.Round(bmi, 2);
                if (bmi < 18.5)
                    healthCheck.NutritionStatus = "Underweight";
                else if (bmi < 25)
                    healthCheck.NutritionStatus = "Normal";
                else if (bmi < 30)
                    healthCheck.NutritionStatus = "Overweight";
                else if (bmi < 40)
                    healthCheck.NutritionStatus = "Obese";
                else
                    healthCheck.NutritionStatus = "ExtremlyObese";
            }
            else
            {
                healthCheck.BMI = null;
                healthCheck.NutritionStatus = null;
            }
        }
    }
}
