using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IHealthRecordRepo
    {
        Task<HealthRecord> CreateHealthRecordAsync(HealthRecord healthRecord);
        Task<HealthRecord?> GetHealthRecordByIdAsync(int id);
        Task<List<HealthRecord>> GetAllHealthRecordsAsync();
        Task<List<HealthRecord>> GetHealthRecordsByStudentIdAsync(int studentId);
        Task<HealthRecord?> UpdateHealthRecordAsync(HealthRecord healthRecord);
        Task<bool> DeleteHealthRecordAsync(int id);
    }
}