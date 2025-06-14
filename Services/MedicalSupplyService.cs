using BOs.Models;
using Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class MedicalSupplyService : IMedicalSupplyService
    {
        private readonly IMedicalSupplyRepo _repo;
        public MedicalSupplyService(IMedicalSupplyRepo repo) { _repo = repo; }
        public Task<List<MedicalSupply>> GetAllAsync() => _repo.GetAllAsync();
        public Task<MedicalSupply?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<MedicalSupply> CreateAsync(MedicalSupply supply) => _repo.CreateAsync(supply);
        public Task<bool> UpdateAsync(MedicalSupply supply) => _repo.UpdateAsync(supply);
        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}