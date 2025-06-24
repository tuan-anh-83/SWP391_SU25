using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAOs
{
    public class MedicationDAO
    {
        private static MedicationDAO instance = null;
        private readonly DataContext _context;

        private MedicationDAO()
        {
            _context = new DataContext();
        }

        public static MedicationDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MedicationDAO();
                }
                return instance;
            }
        }

        public async Task<List<Medication>> GetAllAsync()
        {
            return await _context.Medications.AsNoTracking().ToListAsync();
        }

        public async Task<Medication?> GetByIdAsync(int id)
        {
            return await _context.Medications.AsNoTracking().FirstOrDefaultAsync(m => m.MedicationId == id);
        }

        public async Task<Medication> CreateAsync(Medication medication)
        {
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
            return medication;
        }

        public async Task<bool> UpdateAsync(Medication medication)
        {
            var existing = await _context.Medications.FindAsync(medication.MedicationId);
            if (existing == null) return false;

            existing.Name = medication.Name;
            existing.Type = medication.Type;
            existing.Usage = medication.Usage;
            existing.ExpiredDate = medication.ExpiredDate;
            existing.Quantity = medication.Quantity;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null) return false;

            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}