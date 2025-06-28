using BOs.Models;
using Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepo _classRepo;

        public ClassService(IClassRepo classRepo)
        {
            _classRepo = classRepo;
        }

        public Task<List<Class>> GetAllClassesAsync() => _classRepo.GetAllClassesAsync();
        public Task<Class?> GetClassByIdAsync(int id) => _classRepo.GetClassByIdAsync(id);
        public Task<Class> CreateClassAsync(Class cls) => _classRepo.CreateClassAsync(cls);
        public Task<bool> UpdateClassAsync(Class cls) => _classRepo.UpdateClassAsync(cls);
        public Task<bool> DeleteClassAsync(int id) => _classRepo.DeleteClassAsync(id);
        public Task<bool> ClassNameExistsAsync(string className, int? excludeId = null)
            => _classRepo.ClassNameExistsAsync(className, excludeId);
    }
}