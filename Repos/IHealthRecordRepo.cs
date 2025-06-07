using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IHealthRecordRepo
    {
        Task<HealthRecord> CreateHealthRecord(HealthRecord healthRecord);
        Task<HealthRecord> GetHealthRecordById(int id);
        Task<List<HealthRecord>> GetAllHealthRecords();
        Task<List<HealthRecord>> GetHealthRecordsByStudentId(int studentId);
        Task<HealthRecord> UpdateHealthRecord(HealthRecord healthRecord);
        Task<bool> DeleteHealthRecord(int id);
    }
} 