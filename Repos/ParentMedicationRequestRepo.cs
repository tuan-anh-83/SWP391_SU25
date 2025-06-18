using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class ParentMedicationRequestRepo : IParentMedicationRequestRepo
    {
        public Task<bool> CreateAsync(ParentMedicationRequest request) 
            => ParentMedicationRequestDAO.Instance.CreateAsync(request);

        public Task<List<ParentMedicationRequest>> GetAllAsync() 
            => ParentMedicationRequestDAO.Instance.GetAllAsync();

        public Task<ParentMedicationRequest?> GetByIdAsync(int id) 
            => ParentMedicationRequestDAO.Instance.GetByIdAsync(id);

        public Task<bool> ApproveAsync(int id, string status, string nurseNote) 
            => ParentMedicationRequestDAO.Instance.ApproveAsync(id, status, nurseNote);

        public Task<bool> UpdateAsync(ParentMedicationRequest request)
            => ParentMedicationRequestDAO.Instance.UpdateAsync(request);

        public Task<List<ParentMedicationRequest>> GetByParentIdAsync(int parentId)
            => ParentMedicationRequestDAO.Instance.GetByParentIdAsync(parentId);
    }
}