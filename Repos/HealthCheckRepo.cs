using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class HealthCheckRepo : IHealthCheckRepo
    {
        private readonly HealthCheckDAO _healthCheckDAO;

        public HealthCheckRepo()
        {
            _healthCheckDAO = HealthCheckDAO.Instance;
        }

        public async Task<List<HealthCheck>> GetAllHealthChecksAsync()
        {
            return await _healthCheckDAO.GetAllHealthChecksAsync();
        }

        public async Task<HealthCheck> GetHealthCheckByIdAsync(int id)
        {
            return await _healthCheckDAO.GetHealthCheckByIdAsync(id);
        }

        public async Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId)
        {
            return await _healthCheckDAO.GetHealthChecksByStudentIdAsync(studentId);
        }

        public async Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId)
        {
            return await _healthCheckDAO.GetHealthChecksByNurseIdAsync(nurseId);
        }

        public async Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId)
        {
            return await _healthCheckDAO.GetHealthChecksByParentIdAsync(parentId);
        }

        public async Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck)
        {
            return await _healthCheckDAO.CreateHealthCheckAsync(healthCheck);
        }

        public async Task<HealthCheck> UpdateHealthCheckAsync(HealthCheck healthCheck)
        {
            return await _healthCheckDAO.UpdateHealthCheckAsync(healthCheck);
        }

        public async Task<bool> DeleteHealthCheckAsync(int id)
        {
            return await _healthCheckDAO.DeleteHealthCheckAsync(id);
        }

        public async Task<bool> ConfirmHealthCheckAsync(int healthCheckId)
        {
            return await _healthCheckDAO.ConfirmHealthCheckAsync(healthCheckId);
        }

        public async Task<bool> DeclineHealthCheckAsync(int healthCheckId)
        {
            return await _healthCheckDAO.DeclineHealthCheckAsync(healthCheckId);
        }
    }
}
