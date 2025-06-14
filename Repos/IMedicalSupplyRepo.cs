using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IMedicalSupplyRepo
    {
        Task<List<MedicalSupply>> GetAllAsync();
        Task<MedicalSupply?> GetByIdAsync(int id);
        Task<MedicalSupply> CreateAsync(MedicalSupply supply);
        Task<bool> UpdateAsync(MedicalSupply supply);
        Task<bool> DeleteAsync(int id);
    }
}