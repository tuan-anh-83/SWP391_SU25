using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IMedicalSupplyService
    {
        Task<List<MedicalSupply>> GetAllAsync();
        Task<MedicalSupply?> GetByIdAsync(int id);
        Task<MedicalSupply> CreateAsync(MedicalSupply supply);
        Task<bool> UpdateAsync(MedicalSupply supply);
        Task<bool> DeleteAsync(int id);
    }
}