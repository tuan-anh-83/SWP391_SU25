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

        public async Task<List<MedicalEvent>> GetAllMedicalEventsAsync()
        {
            return await _repo.GetAllMedicalEventsAsync();
        }

        public async Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId)
        {
            return await _repo.GetMedicalEventByIdAsync(eventId);
        }

        public async Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<int> medicationIds)
        {
            return await _repo.CreateMedicalEventAsync(medicalEvent, medicationIds);
        }

        public async Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds)
        {
            return await _repo.UpdateMedicalEventAsync(medicalEvent, medicationIds);
        }

        public async Task<bool> DeleteMedicalEventAsync(int eventId)
        {
            return await _repo.DeleteMedicalEventAsync(eventId);
        }
    }
}