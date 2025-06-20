using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class MedicalEventRepo : IMedicalEventRepo
    {
        private readonly MedicalEventDAO _dao = MedicalEventDAO.Instance;

        public Task<List<MedicalEvent>> GetAllMedicalEventsAsync() => _dao.GetAllMedicalEventsAsync();
        public Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId) => _dao.GetMedicalEventByIdAsync(eventId);
        public Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<MedicalEventMedication> medicationUsages, List<MedicalEventMedicalSupply> supplyUsages)
            => _dao.CreateMedicalEventAsync(medicalEvent, medicationUsages, supplyUsages);
        public Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<MedicalEventMedication> medicationUsages, List<MedicalEventMedicalSupply> supplyUsages)
            => _dao.UpdateMedicalEventAsync(medicalEvent, medicationUsages, supplyUsages);
        public Task<bool> DeleteMedicalEventAsync(int eventId) => _dao.DeleteMedicalEventAsync(eventId);
        public Task<List<(Student student, List<MedicalEvent> events)>> GetMedicalEventsByParentIdAsync(int parentId)
            => _dao.GetMedicalEventsByParentIdAsync(parentId);
    }
}