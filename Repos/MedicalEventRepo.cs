using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class MedicalEventRepo : IMedicalEventRepo
    {
        public async Task<List<MedicalEvent>> GetAllMedicalEventsAsync()
        {
            return await MedicalEventDAO.Instance.GetAllMedicalEventsAsync();
        }

        public async Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId)
        {
            return await MedicalEventDAO.Instance.GetMedicalEventByIdAsync(eventId);
        }

        public async Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<int> medicationIds)
        {
            return await MedicalEventDAO.Instance.CreateMedicalEventAsync(medicalEvent, medicationIds);
        }

        public async Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds)
        {
            return await MedicalEventDAO.Instance.UpdateMedicalEventAsync(medicalEvent, medicationIds);
        }

        public async Task<bool> DeleteMedicalEventAsync(int eventId)
        {
            return await MedicalEventDAO.Instance.DeleteMedicalEventAsync(eventId);
        }
    }
}