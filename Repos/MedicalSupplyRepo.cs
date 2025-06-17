using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class MedicalSupplyRepo : IMedicalSupplyRepo
    {
        public Task<List<MedicalSupply>> GetAllAsync() => MedicalSupplyDAO.Instance.GetAllAsync();
        public Task<MedicalSupply?> GetByIdAsync(int id) => MedicalSupplyDAO.Instance.GetByIdAsync(id);
        public Task<MedicalSupply> CreateAsync(MedicalSupply supply) => MedicalSupplyDAO.Instance.CreateAsync(supply);
        public Task<bool> UpdateAsync(MedicalSupply supply) => MedicalSupplyDAO.Instance.UpdateAsync(supply);
        public Task<bool> DeleteAsync(int id) => MedicalSupplyDAO.Instance.DeleteAsync(id);
    }
}