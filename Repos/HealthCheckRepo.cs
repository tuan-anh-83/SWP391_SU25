using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class HealthCheckRepo : IHealthCheckRepo
    {
        public async Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck)
            => await HealthCheckDAO.Instance.CreateHealthCheckAsync(healthCheck);

        public async Task<HealthCheck?> GetHealthCheckByIdAsync(int id)
            => await HealthCheckDAO.Instance.GetHealthCheckByIdAsync(id);

        public async Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId)
            => await HealthCheckDAO.Instance.GetHealthChecksByStudentIdAsync(studentId);

        public async Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId)
            => await HealthCheckDAO.Instance.GetHealthChecksByParentIdAsync(parentId);

        public async Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId)
            => await HealthCheckDAO.Instance.GetHealthChecksByNurseIdAsync(nurseId);

        public async Task<List<HealthCheck>> GetAllHealthChecksAsync()
            => await HealthCheckDAO.Instance.GetAllHealthChecksAsync();

        public async Task<HealthCheck?> UpdateHealthCheckAsync(HealthCheck healthCheck)
            => await HealthCheckDAO.Instance.UpdateHealthCheckAsync(healthCheck);

        public async Task<bool> DeleteHealthCheckAsync(int id)
            => await HealthCheckDAO.Instance.DeleteHealthCheckAsync(id);

        public async Task<List<HealthCheck>> GetHealthChecksByDateAsync(DateTime date)
            => await HealthCheckDAO.Instance.GetHealthChecksByDateAsync(date);

        public async Task<int> UpdateParentForFutureHealthChecksAsync(int studentId, int parentId)
            => await HealthCheckDAO.Instance.UpdateParentForFutureHealthChecksAsync(studentId, parentId);
    }
}
