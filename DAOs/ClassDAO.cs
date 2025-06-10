using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAOs
{
    public class ClassDAO
    {
        private static ClassDAO instance = null;
        private readonly DataContext _context;

        private ClassDAO()
        {
            _context = new DataContext();
        }

        public static ClassDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClassDAO();
                }
                return instance;
            }
        }

        public async Task<List<Class>> GetAllClassesAsync()
        {
            return await _context.Classes.ToListAsync();
        }

        public async Task<Class?> GetClassByIdAsync(int id)
        {
            return await _context.Classes.FirstOrDefaultAsync(c => c.ClassId == id);
        }

        public async Task<Class> CreateClassAsync(Class cls)
        {
            _context.Classes.Add(cls);
            await _context.SaveChangesAsync();
            return cls;
        }

        public async Task<bool> UpdateClassAsync(Class cls)
        {
            var existing = await _context.Classes.FindAsync(cls.ClassId);
            if (existing == null) return false;
            existing.ClassName = cls.ClassName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClassAsync(int id)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls == null) return false;
            _context.Classes.Remove(cls);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}