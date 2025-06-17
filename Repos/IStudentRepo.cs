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
        Task<List<Student>> GetStudentsByParentIdAsync(int parentId);
        Task<List<Student>> GetStudentsByClassIdAsync(int classId);
        Task<List<Student>> GetStudentsByClassIdsAsync(List<int> classIds);
    }
}
