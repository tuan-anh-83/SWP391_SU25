using BOs.Models;

namespace Services
{
    public interface IHealthRecordService
    {
        Task<HealthRecord> CreateHealthRecordAsync(HealthRecord healthRecord);
        Task<HealthRecord?> GetHealthRecordByIdAsync(int id);
        Task<List<HealthRecord>> GetAllHealthRecordsAsync();
        Task<List<HealthRecord>> GetHealthRecordsByStudentIdAsync(int studentId);
        Task<HealthRecord?> UpdateHealthRecordAsync(HealthRecord healthRecord);
        Task<bool> DeleteHealthRecordAsync(int id);
    }
}