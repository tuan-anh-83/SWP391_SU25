using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IClassService
    {
        Task<List<Class>> GetAllClassesAsync();
        Task<Class?> GetClassByIdAsync(int id);
        Task<Class> CreateClassAsync(Class cls);
        Task<bool> UpdateClassAsync(Class cls);
        Task<bool> DeleteClassAsync(int id);
    }
}