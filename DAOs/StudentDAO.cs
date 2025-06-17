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

        public async Task<Student> CreateStudentAsync(Student student)
        {
            student.CreatedAt = DateTime.UtcNow;
            student.UpdateAt = DateTime.UtcNow;
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Parent)
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Parent)
                .Include(s => s.Class)
                .ToListAsync();
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            var existing = await _context.Students.FindAsync(student.StudentId);
            if (existing == null) return false;

            // Update fields
            existing.Fullname = student.Fullname;
            existing.ClassId = student.ClassId;
            existing.StudentCode = student.StudentCode;
            existing.Gender = student.Gender;
            existing.ParentId = student.ParentId;
            existing.DateOfBirth = student.DateOfBirth;
            existing.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Student>> GetStudentsByParentIdAsync(int parentId)
        {
            return await _context.Students
                .Include(s => s.Class)
                .Where(s => s.ParentId == parentId)
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByClassIdAsync(int classId)
        {
            return await _context.Students
                .Where(s => s.ClassId == classId)
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByClassIdsAsync(List<int> classIds)
        {
            return await _context.Students
                .Where(s => classIds.Contains(s.ClassId))
                .ToListAsync();
        }
    }
}
