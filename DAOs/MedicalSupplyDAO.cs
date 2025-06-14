using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAOs
{
    public class MedicalSupplyDAO
    {
        private static MedicalSupplyDAO instance = null;
        private readonly DataContext _context;

        private MedicalSupplyDAO()
        {
            _context = new DataContext();
        }

        public static MedicalSupplyDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MedicalSupplyDAO();
                }
                return instance;
            }
        }

        public async Task<List<MedicalSupply>> GetAllAsync()
        {
            return await _context.MedicalSupplies.AsNoTracking().ToListAsync();
        }

        public async Task<MedicalSupply?> GetByIdAsync(int id)
        {
            return await _context.MedicalSupplies.FindAsync(id);
        }

        public async Task<MedicalSupply> CreateAsync(MedicalSupply supply)
        {
            await _context.MedicalSupplies.AddAsync(supply);
            await _context.SaveChangesAsync();
            return supply;
        }

        public async Task<bool> UpdateAsync(MedicalSupply supply)
        {
            _context.MedicalSupplies.Update(supply);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var supply = await _context.MedicalSupplies.FindAsync(id);
            if (supply == null) return false;
            _context.MedicalSupplies.Remove(supply);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}