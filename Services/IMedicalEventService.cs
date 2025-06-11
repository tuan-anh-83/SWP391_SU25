using BOs.Models;

namespace Services
{
    public interface IMedicalEventService
    {
        Task<List<MedicalEvent>> GetAllMedicalEventsAsync();
        Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId);
        Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<int> medicationIds);
        Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds);
        Task<bool> DeleteMedicalEventAsync(int eventId);
    }
}