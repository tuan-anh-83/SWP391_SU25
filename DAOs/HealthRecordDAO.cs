using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class HealthRecordDAO
    {
        private static HealthRecordDAO instance = null;
        private readonly DataContext _context;

        private HealthRecordDAO()
        {
            _context = new DataContext();
        }

        public static HealthRecordDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new HealthRecordDAO();
                }
                return instance;
            }
        }

        public async Task<HealthRecord> CreateHealthRecordAsync(HealthRecord healthRecord)
        {
            _context.HealthRecords.Add(healthRecord);
            await _context.SaveChangesAsync();
            return healthRecord;
        }

        public async Task<HealthRecord?> GetHealthRecordByIdAsync(int id)
        {
            return await _context.HealthRecords
                .Include(hr => hr.Student)
                .Include(hr => hr.Parent)
                .FirstOrDefaultAsync(hr => hr.HealthRecordId == id);
        }

        public async Task<List<HealthRecord>> GetAllHealthRecordsAsync()
        {
            return await _context.HealthRecords
                .Include(hr => hr.Student)
                .Include(hr => hr.Parent)
                .ToListAsync();
        }

        public async Task<List<HealthRecord>> GetHealthRecordsByStudentIdAsync(int studentId)
        {
            return await _context.HealthRecords
                .Where(hr => hr.StudentId == studentId)
                .Include(hr => hr.Student)
                .Include(hr => hr.Parent)
                .ToListAsync();
        }

        public async Task<HealthRecord?> UpdateHealthRecordAsync(HealthRecord healthRecord)
        {
            var existing = await _context.HealthRecords.FindAsync(healthRecord.HealthRecordId);
            if (existing == null) return null;

            existing.Height = healthRecord.Height;
            existing.Weight = healthRecord.Weight;
            //existing.BMI = healthRecord.BMI;
            //existing.NutritionStatus = healthRecord.NutritionStatus;
            existing.LeftEye = healthRecord.LeftEye;
            existing.RightEye = healthRecord.RightEye;
            existing.Note = healthRecord.Note;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteHealthRecordAsync(int id)
        {
            var record = await _context.HealthRecords.FindAsync(id);
            if (record == null) return false;
            _context.HealthRecords.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}