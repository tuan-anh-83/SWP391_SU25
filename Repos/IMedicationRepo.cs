using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IMedicationRepo
    {
        Task<List<Medication>> GetAllAsync();
        Task<Medication?> GetByIdAsync(int id);
        Task<Medication> CreateAsync(Medication medication);
        Task<bool> UpdateAsync(Medication medication);
        Task<bool> DeleteAsync(int id);
    }
}