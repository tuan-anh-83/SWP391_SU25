using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class HealthRecordRepo : IHealthRecordRepo
    {
        public Task<HealthRecord> CreateHealthRecordAsync(HealthRecord healthRecord)
            => HealthRecordDAO.Instance.CreateHealthRecordAsync(healthRecord);

        public Task<HealthRecord?> GetHealthRecordByIdAsync(int id)
            => HealthRecordDAO.Instance.GetHealthRecordByIdAsync(id);

        public Task<List<HealthRecord>> GetAllHealthRecordsAsync()
            => HealthRecordDAO.Instance.GetAllHealthRecordsAsync();

        public Task<List<HealthRecord>> GetHealthRecordsByStudentIdAsync(int studentId)
            => HealthRecordDAO.Instance.GetHealthRecordsByStudentIdAsync(studentId);

        public Task<HealthRecord?> UpdateHealthRecordAsync(HealthRecord healthRecord)
            => HealthRecordDAO.Instance.UpdateHealthRecordAsync(healthRecord);

        public Task<bool> DeleteHealthRecordAsync(int id)
            => HealthRecordDAO.Instance.DeleteHealthRecordAsync(id);
    }
}