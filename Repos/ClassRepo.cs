using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class ClassRepo : IClassRepo
    {
        public Task<List<Class>> GetAllClassesAsync() => ClassDAO.Instance.GetAllClassesAsync();
        public Task<Class?> GetClassByIdAsync(int id) => ClassDAO.Instance.GetClassByIdAsync(id);
        public Task<Class> CreateClassAsync(Class cls) => ClassDAO.Instance.CreateClassAsync(cls);
        public Task<bool> UpdateClassAsync(Class cls) => ClassDAO.Instance.UpdateClassAsync(cls);
        public Task<bool> DeleteClassAsync(int id) => ClassDAO.Instance.DeleteClassAsync(id);
        public Task<bool> ClassNameExistsAsync(string className, int? excludeId = null)
            => ClassDAO.Instance.ClassNameExistsAsync(className, excludeId);
    }
}   