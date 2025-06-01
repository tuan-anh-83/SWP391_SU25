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
    }

}
