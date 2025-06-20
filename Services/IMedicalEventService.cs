using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IMedicalEventService
    {
        Task<List<MedicalEvent>> GetAllMedicalEventsAsync();
        Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId);
        Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<MedicalEventMedication> medicationUsages, List<MedicalEventMedicalSupply> supplyUsages);
        Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<MedicalEventMedication> medicationUsages, List<MedicalEventMedicalSupply> supplyUsages);
        Task<bool> DeleteMedicalEventAsync(int eventId);
        Task<List<(Student student, List<MedicalEvent> events)>> GetMedicalEventsByParentIdAsync(int parentId);
    }
}