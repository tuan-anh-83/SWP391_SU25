using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IParentMedicationRequestRepo
    {
        Task<bool> CreateAsync(ParentMedicationRequest request, List<int> medicationIds);
        Task<List<ParentMedicationRequest>> GetAllAsync();
        Task<ParentMedicationRequest?> GetByIdAsync(int id);
        Task<bool> ApproveAsync(int id, string status, string? note);
    }
}