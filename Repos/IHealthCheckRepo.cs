using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IHealthCheckRepo
    {
        Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck);
        Task<HealthCheck?> GetHealthCheckByIdAsync(int id);
        Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId);
        Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId);
        Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId);
        Task<List<HealthCheck>> GetAllHealthChecksAsync();
        Task<HealthCheck?> UpdateHealthCheckAsync(HealthCheck healthCheck);
        Task<bool> DeleteHealthCheckAsync(int id);
        Task<List<HealthCheck>> GetHealthChecksByDateAsync(DateTime date);
        Task<int> UpdateParentForFutureHealthChecksAsync(int studentId, int parentId);
    }
}
