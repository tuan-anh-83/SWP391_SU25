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
        public Task<bool> CreateAsync(ParentMedicationRequest request, List<int> medicationIds) => _repo.CreateAsync(request, medicationIds);
        public Task<List<ParentMedicationRequest>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ParentMedicationRequest?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<bool> ApproveAsync(int id, string status, string? note) => _repo.ApproveAsync(id, status, note);
    }
}