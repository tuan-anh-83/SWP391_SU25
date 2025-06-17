using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IHealthCheckRepo
    {
        Task<List<HealthCheck>> GetAllHealthChecksAsync();
        Task<HealthCheck> GetHealthCheckByIdAsync(int id);
        Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId);
        Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId);
        Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId);
        Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck);
        Task<HealthCheck> UpdateHealthCheckAsync(HealthCheck healthCheck);
        Task<bool> DeleteHealthCheckAsync(int id);
        Task<bool> ConfirmHealthCheckAsync(int healthCheckId);
        Task<bool> DeclineHealthCheckAsync(int healthCheckId);
    }
}
