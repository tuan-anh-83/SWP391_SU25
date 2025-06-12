using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class ParentMedicationRequestRepo : IParentMedicationRequestRepo
    {
        public Task<bool> CreateAsync(ParentMedicationRequest request, List<int> medicationIds) => ParentMedicationRequestDAO.Instance.CreateAsync(request, medicationIds);
        public Task<List<ParentMedicationRequest>> GetAllAsync() => ParentMedicationRequestDAO.Instance.GetAllAsync();
        public Task<ParentMedicationRequest?> GetByIdAsync(int id) => ParentMedicationRequestDAO.Instance.GetByIdAsync(id);
        public Task<bool> ApproveAsync(int id, string status, string? note) => ParentMedicationRequestDAO.Instance.ApproveAsync(id, status, note);
    }
}