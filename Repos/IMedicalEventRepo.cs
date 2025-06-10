using BOs.Models;

namespace Repos
{
    public interface IMedicalEventRepo
    {
        Task<List<MedicalEvent>> GetAllMedicalEventsAsync();
        Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId);
        Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent);
        Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent);
        Task<bool> DeleteMedicalEventAsync(int eventId);
    }
}