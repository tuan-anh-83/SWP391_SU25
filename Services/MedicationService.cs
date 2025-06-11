using BOs.Models;
using Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationRepo _repo;

        public MedicationService(IMedicationRepo repo)
        {
            _repo = repo;
        }

        public Task<List<Medication>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Medication?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<Medication> CreateAsync(Medication medication) => _repo.CreateAsync(medication);
        public Task<bool> UpdateAsync(Medication medication) => _repo.UpdateAsync(medication);
        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}