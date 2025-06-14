using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IMedicalEventRepo
    {
        Task<List<MedicalEvent>> GetAllMedicalEventsAsync();
        Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId);
        Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds, List<int>? medicalSupplyIds);
        Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds, List<int>? medicalSupplyIds);
        Task<bool> DeleteMedicalEventAsync(int eventId);
    }
}