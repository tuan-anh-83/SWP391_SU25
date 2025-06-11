using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public interface IStudentRepo
    {
        Task<Student?> GetStudentByIdAsync(int id);
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student> CreateStudentAsync(Student student);
        Task<bool> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);
        Task<Student?> GetStudentByCodeAsync(string studentCode);
        Task<bool> AssignParentToStudentAsync(string studentCode, int parentId);
    }
}
