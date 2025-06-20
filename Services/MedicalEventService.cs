using BOs.Models;
using Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class MedicalEventService : IMedicalEventService
    {
        private readonly IMedicalEventRepo _repo;

        public MedicalEventService(IMedicalEventRepo repo)
        {
            _repo = repo;
        }

        public Task<List<MedicalEvent>> GetAllMedicalEventsAsync() => _repo.GetAllMedicalEventsAsync();
        public Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId) => _repo.GetMedicalEventByIdAsync(eventId);
        public Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<MedicalEventMedication> medicationUsages, List<MedicalEventMedicalSupply> supplyUsages)
            => _repo.CreateMedicalEventAsync(medicalEvent, medicationUsages, supplyUsages);
        public Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<MedicalEventMedication> medicationUsages, List<MedicalEventMedicalSupply> supplyUsages)
            => _repo.UpdateMedicalEventAsync(medicalEvent, medicationUsages, supplyUsages);
        public Task<bool> DeleteMedicalEventAsync(int eventId) => _repo.DeleteMedicalEventAsync(eventId);
        public Task<List<(Student student, List<MedicalEvent> events)>> GetMedicalEventsByParentIdAsync(int parentId)
            => _repo.GetMedicalEventsByParentIdAsync(parentId);
    }
}