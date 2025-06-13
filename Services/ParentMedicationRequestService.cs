using BOs.Models;
using Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ParentMedicationRequestService : IParentMedicationRequestService
    {
        private readonly IParentMedicationRequestRepo _repo;
        public ParentMedicationRequestService(IParentMedicationRequestRepo repo) { _repo = repo; }
        public Task<bool> CreateAsync(ParentMedicationRequest request) => _repo.CreateAsync(request);
        public Task<List<ParentMedicationRequest>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ParentMedicationRequest?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<bool> ApproveAsync(int id, string status, string nurseNote) => _repo.ApproveAsync(id, status, nurseNote);
    }
}