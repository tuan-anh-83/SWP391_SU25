using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepo _healthCheckRepo;

        public HealthCheckService(IHealthCheckRepo healthCheckRepo)
        {
            _healthCheckRepo = healthCheckRepo;
        }

        public async Task<List<HealthCheck>> GetAllHealthChecksAsync()
        {
            try
            {
                return await _healthCheckRepo.GetAllHealthChecksAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving health checks", ex);
            }
        }

        public async Task<HealthCheck> GetHealthCheckByIdAsync(int id)
        {
            try
            {
                var healthCheck = await _healthCheckRepo.GetHealthCheckByIdAsync(id);
                return healthCheck;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving health check with ID {id}", ex);
            }
        }

        public async Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId)
        {
            try
            {
                return await _healthCheckRepo.GetHealthChecksByStudentIdAsync(studentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving health checks for student {studentId}", ex);
            }
        }

        public async Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId)
        {
            try
            {
                return await _healthCheckRepo.GetHealthChecksByNurseIdAsync(nurseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving health checks for nurse {nurseId}", ex);
            }
        }

        public async Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId)
        {
            try
            {
                return await _healthCheckRepo.GetHealthChecksByParentIdAsync(parentId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving health checks for parent {parentId}", ex);
            }
        }

        public async Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck)
        {
            try
            {
                if (healthCheck == null)
                {
                    throw new ArgumentNullException(nameof(healthCheck));
                }

                // Basic data integrity validation
                if (healthCheck.StudentID == null || healthCheck.NurseID == null || healthCheck.ParentID <= 0)
                {
                    throw new ArgumentException("Invalid health check data: StudentID, NurseID, and ParentID are required");
                }

                healthCheck.Date = DateTime.Now;
                healthCheck.ConfirmByParent = null; // Initially null until parent confirms/declines

                return await _healthCheckRepo.CreateHealthCheckAsync(healthCheck);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating health check", ex);
            }
        }

        public async Task<HealthCheck> UpdateHealthCheckAsync(HealthCheck healthCheck)
        {
            try
            {
              

                var existingHealthCheck = await _healthCheckRepo.GetHealthCheckByIdAsync(healthCheck.HealthCheckID);
                if (existingHealthCheck == null)
                {
                    throw new Exception($"Health check with ID {healthCheck.HealthCheckID} not found");
                }

                return await _healthCheckRepo.UpdateHealthCheckAsync(healthCheck);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating health check with ID {healthCheck?.HealthCheckID}", ex);
            }
        }

        public async Task<bool> DeleteHealthCheckAsync(int id)
        {
            try
            {
                var healthCheck = await _healthCheckRepo.GetHealthCheckByIdAsync(id);

                return await _healthCheckRepo.DeleteHealthCheckAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting health check with ID {id}", ex);
            }
        }

        public async Task<bool> ConfirmHealthCheckAsync(int healthCheckId)
        {
            try
            {
                var healthCheck = await _healthCheckRepo.GetHealthCheckByIdAsync(healthCheckId);
                return await _healthCheckRepo.ConfirmHealthCheckAsync(healthCheckId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error confirming health check with ID {healthCheckId}", ex);
            }
        }

        public async Task<bool> DeclineHealthCheckAsync(int healthCheckId)
        {
            try
            {
                var healthCheck = await _healthCheckRepo.GetHealthCheckByIdAsync(healthCheckId);
                return await _healthCheckRepo.DeclineHealthCheckAsync(healthCheckId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error declining health check with ID {healthCheckId}", ex);
            }
        }
    }
}
