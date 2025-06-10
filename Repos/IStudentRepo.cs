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
        Task<Student?> GetStudentByCodeAsync(string studentCode);
        Task<bool> AssignParentToStudentAsync(string studentCode, int parentId);
        Task<Student> CreateStudentAsync(Student student);
    }
}
