using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class MedicalEventRepo : IMedicalEventRepo
    {
        public Task<List<MedicalEvent>> GetAllMedicalEventsAsync() => MedicalEventDAO.Instance.GetAllMedicalEventsAsync();
        public Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId) => MedicalEventDAO.Instance.GetMedicalEventByIdAsync(eventId);
        public Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds, List<int>? medicalSupplyIds)
            => MedicalEventDAO.Instance.CreateMedicalEventAsync(medicalEvent, medicationIds, medicalSupplyIds);
        public Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds, List<int>? medicalSupplyIds)
            => MedicalEventDAO.Instance.UpdateMedicalEventAsync(medicalEvent, medicationIds, medicalSupplyIds);
        public Task<bool> DeleteMedicalEventAsync(int eventId) => MedicalEventDAO.Instance.DeleteMedicalEventAsync(eventId);
        public Task<List<(Student student, List<MedicalEvent> events)>> GetMedicalEventsByParentIdAsync(int parentId)
            => MedicalEventDAO.Instance.GetMedicalEventsByParentIdAsync(parentId);
    }
}