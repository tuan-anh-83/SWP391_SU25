using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class StudentDAO
    {
        private static StudentDAO instance = null;
        private readonly DataContext _context;

        private StudentDAO()
        {
            _context = new DataContext();
        }

        public static StudentDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StudentDAO();
                }
                return instance;
            }
        }

        public async Task<Student?> GetStudentByCodeAsync(string studentCode)
        {
            return await _context.Students
                                 .Include(s => s.Class) 
                                 .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
        }

        public async Task<bool> AssignParentToStudentAsync(string studentCode, int parentId)
        {
            var student = await _context.Students
                                        .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null || student.ParentId != null)
                return false;

            student.ParentId = parentId;
            student.UpdateAt = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

    }

}
