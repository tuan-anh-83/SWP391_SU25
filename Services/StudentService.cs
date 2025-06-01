using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepo _studentRepo;

        public StudentService(IStudentRepo studentRepo)
        {
            _studentRepo = studentRepo;
        }

        public async Task<Student?> GetStudentByCodeAsync(string studentCode)
        {
            return await _studentRepo.GetStudentByCodeAsync(studentCode);
        }

        public async Task<bool> LinkStudentToParentAsync(string studentCode, int parentId)
        {
            return await _studentRepo.AssignParentToStudentAsync(studentCode, parentId);
        }
    }

}
