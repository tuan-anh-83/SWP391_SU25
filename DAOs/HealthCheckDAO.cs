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

        // Get all health checks
        public async Task<List<HealthCheck>> GetAllHealthChecksAsync()
        {
            return await _context.HealthChecks
                .Include(h => h.Student)
                .Include(h => h.Nurse)
                .Include(h => h.Parent)
                .ToListAsync();
        }

        // Get health check by ID
        public async Task<HealthCheck> GetHealthCheckByIdAsync(int id)
        {
            return await _context.HealthChecks
                .Include(h => h.Student)
                .Include(h => h.Nurse)
                .Include(h => h.Parent)
                .FirstOrDefaultAsync(h => h.HealthCheckID == id);
        }

        // Get health checks by student ID
        public async Task<List<HealthCheck>> GetHealthChecksByStudentIdAsync(int studentId)
        {
            return await _context.HealthChecks
                .Include(h => h.Student)
                .Include(h => h.Nurse)
                .Include(h => h.Parent)
                .Where(h => h.StudentID == studentId)
                .ToListAsync();
        }

        // Get health checks by nurse ID
        public async Task<List<HealthCheck>> GetHealthChecksByNurseIdAsync(int nurseId)
        {
            return await _context.HealthChecks
                .Include(h => h.Student)
                .Include(h => h.Nurse)
                .Include(h => h.Parent)
                .Where(h => h.NurseID == nurseId)
                .ToListAsync();
        }

        // Get health checks by parent ID
        public async Task<List<HealthCheck>> GetHealthChecksByParentIdAsync(int parentId)
        {
            return await _context.HealthChecks
                .Include(h => h.Student)
                .Include(h => h.Nurse)
                .Include(h => h.Parent)
                .Where(h => h.ParentID == parentId)
                .ToListAsync();
        }

        // Create new health check
        public async Task<HealthCheck> CreateHealthCheckAsync(HealthCheck healthCheck)
        {
            _context.HealthChecks.Add(healthCheck);
            await _context.SaveChangesAsync();
            return healthCheck;
        }

        // Update health check
        public async Task<HealthCheck> UpdateHealthCheckAsync(HealthCheck healthCheck)
        {
            _context.HealthChecks.Update(healthCheck);
            await _context.SaveChangesAsync();
            return healthCheck;
        }

        // Delete health check
        public async Task<bool> DeleteHealthCheckAsync(int id)
        {
            var healthCheck = await _context.HealthChecks.FindAsync(id);
            if (healthCheck == null)
                return false;

            _context.HealthChecks.Remove(healthCheck);
            await _context.SaveChangesAsync();
            return true;
        }

        // Confirm health check by parent (accept)
        public async Task<bool> ConfirmHealthCheckAsync(int healthCheckId)
        {
            var healthCheck = await _context.HealthChecks.FindAsync(healthCheckId);
            if (healthCheck == null)
                return false;

            healthCheck.ConfirmByParent = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // Decline health check by parent
        public async Task<bool> DeclineHealthCheckAsync(int healthCheckId)
        {
            var healthCheck = await _context.HealthChecks.FindAsync(healthCheckId);
            if (healthCheck == null)
                return false;

            healthCheck.ConfirmByParent = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
