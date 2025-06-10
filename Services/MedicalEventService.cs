using BOs.Models;
using Repos;

namespace Services
{
    public class MedicalEventService : IMedicalEventService
    {
        private readonly IMedicalEventRepo _medicalEventRepo;

        public MedicalEventService(IMedicalEventRepo medicalEventRepo)
        {
            _medicalEventRepo = medicalEventRepo;
        }

        public async Task<List<MedicalEvent>> GetAllMedicalEventsAsync()
        {
            return await _medicalEventRepo.GetAllMedicalEventsAsync();
        }

        public async Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId)
        {
            return await _medicalEventRepo.GetMedicalEventByIdAsync(eventId);
        }

        public async Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent)
        {
            return await _medicalEventRepo.CreateMedicalEventAsync(medicalEvent);
        }

        public async Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent)
        {
            return await _medicalEventRepo.UpdateMedicalEventAsync(medicalEvent);
        }

        public async Task<bool> DeleteMedicalEventAsync(int eventId)
        {
            return await _medicalEventRepo.DeleteMedicalEventAsync(eventId);
        }
    }
}