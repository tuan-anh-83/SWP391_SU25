using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAOs
{
    public class HealthCheckDAO
    {
        private static HealthCheckDAO instance = null;
        private readonly DataContext _context;

        private HealthCheckDAO()
        {
            _context = new DataContext();
        }

        public static HealthCheckDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HealthCheckDAO();
                }
                return instance;
            }
        }

        public async Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck)
        {
            await _context.HealthChecks.AddAsync(healthCheck);
            await _context.SaveChangesAsync();
            return healthCheck;
        }

        public async Task<HealthCheck?> GetHealthCheckByIdAsync(int id)
        {
            return await _context.HealthChecks
                .Include(h => h.Student)
                .Include(h => h.Parent)
                .Include(h => h.Nurse)
                .FirstOrDefaultAsync(h => h.HealthCheckID == id);
        }

        public async Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId)
        {
            return await _context.HealthChecks
                .Where(h => h.StudentID == studentId)
                .Include(h => h.Student)
                .Include(h => h.Parent)
                .Include(h => h.Nurse)
                .ToListAsync();
        }

        public async Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId)
        {
            return await _context.HealthChecks
                .Where(h => h.ParentID == parentId)
                .Include(h => h.Student)
                .Include(h => h.Parent)
                .Include(h => h.Nurse)
                .ToListAsync();
        }

        public async Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId)
        {
            return await _context.HealthChecks
                .Where(h => h.NurseID == nurseId)
                .Include(h => h.Student)
                .Include(h => h.Parent)
                .Include(h => h.Nurse)
                .ToListAsync();
        }

        public async Task<List<HealthCheck>> GetAllHealthChecksAsync()
        {
            return await _context.HealthChecks
                .Include(h => h.Student)
                .Include(h => h.Parent)
                .Include(h => h.Nurse)
                .ToListAsync();
        }

        public async Task<HealthCheck?> UpdateHealthCheckAsync(HealthCheck healthCheck)
        {
            var existing = await _context.HealthChecks.FindAsync(healthCheck.HealthCheckID);
            if (existing == null) return null;

            existing.Result = healthCheck.Result;
            existing.Date = healthCheck.Date;
            existing.Height = healthCheck.Height;
            existing.Weight = healthCheck.Weight;
            existing.BMI = healthCheck.BMI;
            existing.NutritionStatus = healthCheck.NutritionStatus;
            existing.StudentID = healthCheck.StudentID;
            existing.NurseID = healthCheck.NurseID;
            existing.ParentID = healthCheck.ParentID;
            existing.HealthCheckDescription = healthCheck.HealthCheckDescription;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteHealthCheckAsync(int id)
        {
            var healthCheck = await _context.HealthChecks.FindAsync(id);
            if (healthCheck == null) return false;
            _context.HealthChecks.Remove(healthCheck);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<HealthCheck>> GetHealthChecksByDateAsync(DateTime date)
        {
            return await _context.HealthChecks
                .Where(h => h.Date.Date == date.Date)
                .Include(h => h.Student)
                .Include(h => h.Parent)
                .Include(h => h.Nurse)
                .ToListAsync();
        }

        public async Task<int> UpdateParentForFutureHealthChecksAsync(int studentId, int parentId)
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            var futureChecks = await _context.HealthChecks
                .Where(h => h.StudentID == studentId && h.Date >= tomorrow && h.ParentID == null)
                .ToListAsync();
            foreach (var hc in futureChecks)
            {
                hc.ParentID = parentId;
            }
            return await _context.SaveChangesAsync();
        }
    }
}
