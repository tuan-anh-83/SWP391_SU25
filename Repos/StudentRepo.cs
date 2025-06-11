using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class StudentRepo : IStudentRepo
    {
        public async Task<Student?> GetStudentByCodeAsync(string studentCode)
        {
            return await StudentDAO.Instance.GetStudentByCodeAsync(studentCode);
        }

        public async Task<bool> AssignParentToStudentAsync(string studentCode, int parentId)
        {
            return await StudentDAO.Instance.AssignParentToStudentAsync(studentCode, parentId);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            return await StudentDAO.Instance.CreateStudentAsync(student);
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await StudentDAO.Instance.GetStudentByIdAsync(id);
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await StudentDAO.Instance.GetAllStudentsAsync();
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            return await StudentDAO.Instance.UpdateStudentAsync(student);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            return await StudentDAO.Instance.DeleteStudentAsync(id);
        }
    }

}
