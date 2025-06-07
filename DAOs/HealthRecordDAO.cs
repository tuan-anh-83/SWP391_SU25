using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class HealthRecordDAO
    {
        private readonly DataContext _context;

        public HealthRecordDAO(DataContext context)
        {
            _context = context;
        }

        // Create
        public async Task<HealthRecord> CreateHealthRecord(HealthRecord healthRecord)
        {
            try
            {
                _context.HealthRecords.Add(healthRecord);
                await _context.SaveChangesAsync();
                return healthRecord;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating health record: " + ex.Message);
            }
        }

        // Read
        public async Task<HealthRecord> GetHealthRecordById(int id)
        {
            try
            {
                return await _context.HealthRecords
                    .Include(h => h.Student)
                    .Include(h => h.Parent)
                    .FirstOrDefaultAsync(h => h.HealthRecordId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving health record: " + ex.Message);
            }
        }

        public async Task<List<HealthRecord>> GetAllHealthRecords()
        {
            try
            {
                return await _context.HealthRecords
                    .Include(h => h.Student)
                    .Include(h => h.Parent)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving health records: " + ex.Message);
            }
        }

        public async Task<List<HealthRecord>> GetHealthRecordsByStudentId(int studentId)
        {
            try
            {
                return await _context.HealthRecords
                    .Include(h => h.Student)
                    .Include(h => h.Parent)
                    .Where(h => h.StudentId == studentId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving health records by student ID: " + ex.Message);
            }
        }

        // Update
        public async Task<HealthRecord> UpdateHealthRecord(HealthRecord healthRecord)
        {
            try
            {
                _context.Entry(healthRecord).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return healthRecord;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating health record: " + ex.Message);
            }
        }

        // Delete
        public async Task<bool> DeleteHealthRecord(int id)
        {
            try
            {
                var healthRecord = await _context.HealthRecords.FindAsync(id);
                if (healthRecord == null)
                {
                    return false;
                }

                _context.HealthRecords.Remove(healthRecord);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting health record: " + ex.Message);
            }
        }
    }
} 