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

        public async Task<Student> CreateStudentAsync(Student student)
        {
            return await _studentRepo.CreateStudentAsync(student);
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _studentRepo.GetStudentByIdAsync(id);
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _studentRepo.GetAllStudentsAsync();
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            return await _studentRepo.UpdateStudentAsync(student);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            return await _studentRepo.DeleteStudentAsync(id);
        }
    }

}
