using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class MedicationRepo : IMedicationRepo
    {
        public async Task<List<Medication>> GetAllAsync()
        {
            return await MedicationDAO.Instance.GetAllAsync();
        }

        public async Task<Medication?> GetByIdAsync(int id)
        {
            return await MedicationDAO.Instance.GetByIdAsync(id);
        }

        public async Task<Medication> CreateAsync(Medication medication)
        {
            return await MedicationDAO.Instance.CreateAsync(medication);
        }

        public async Task<bool> UpdateAsync(Medication medication)
        {
            return await MedicationDAO.Instance.UpdateAsync(medication);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await MedicationDAO.Instance.DeleteAsync(id);
        }
    }
}