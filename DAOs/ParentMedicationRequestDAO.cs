using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class ParentMedicationRequestDAO
    {
        private static ParentMedicationRequestDAO instance = null;
        private readonly DataContext _context;

        private ParentMedicationRequestDAO()
        {
            _context = new DataContext();
        }

        public static ParentMedicationRequestDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ParentMedicationRequestDAO();
                }
                return instance;
            }
        }

        public async Task<bool> CreateAsync(ParentMedicationRequest request)
        {
            await _context.AddAsync(request);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<ParentMedicationRequest>> GetAllAsync()
        {
            return await _context.Set<ParentMedicationRequest>()
                .Include(r => r.Parent)
                .Include(r => r.Student)
                .Include(r => r.Medications)
                .ToListAsync();
        }

        public async Task<ParentMedicationRequest?> GetByIdAsync(int id)
        {
            return await _context.Set<ParentMedicationRequest>()
                .Include(r => r.Parent)
                .Include(r => r.Student)
                .Include(r => r.Medications)
                .FirstOrDefaultAsync(r => r.RequestId == id);
        }

        public async Task<bool> ApproveAsync(int id, string status, string nurseNote)
        {
            var req = await _context.Set<ParentMedicationRequest>().FindAsync(id);
            if (req == null) return false;
            req.Status = status;
            req.NurseNote = nurseNote;
            _context.Update(req);
            return await _context.SaveChangesAsync() > 0;
        }

        // Thêm API update cho Parent (chỉ khi Pending)
        public async Task<bool> UpdateAsync(ParentMedicationRequest request)
        {
            _context.Update(request);
            return await _context.SaveChangesAsync() > 0;
        }

        // Lấy lịch sử gửi thuốc của Parent
        public async Task<List<ParentMedicationRequest>> GetByParentIdAsync(int parentId)
        {
            return await _context.Set<ParentMedicationRequest>()
                .Include(r => r.Parent)
                .Include(r => r.Student)
                .Include(r => r.Medications)
                .Where(r => r.ParentId == parentId)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();
        }
    }
}