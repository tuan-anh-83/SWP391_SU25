using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class MedicalEventDAO
    {
        private static MedicalEventDAO instance = null;
        private readonly DataContext _context;

        private MedicalEventDAO()
        {
            _context = new DataContext();
        }

        public static MedicalEventDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MedicalEventDAO();
                }
                return instance;
            }
        }

        public async Task<List<MedicalEvent>> GetAllMedicalEventsAsync()
        {
            return await _context.MedicalEvents
                .Include(me => me.Student)
                .Include(me => me.Nurse)
                .Include(me => me.Medications)
                .Include(me => me.MedicalSupplies)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId)
        {
            return await _context.MedicalEvents
                .Include(me => me.Student)
                .Include(me => me.Nurse)
                .Include(me => me.Medications)
                .Include(me => me.MedicalSupplies)
                .AsNoTracking()
                .FirstOrDefaultAsync(me => me.MedicalEventId == eventId);
        }

        public async Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds, List<int>? medicalSupplyIds)
        {
            // Kiểm tra StudentId, NurseId
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == medicalEvent.StudentId);
            var nurseExists = await _context.Accounts.AnyAsync(a => a.AccountID == medicalEvent.NurseId);
            if (!studentExists || !nurseExists)
                return false;

            // Gán danh sách thuốc
            if (medicationIds != null && medicationIds.Count > 0)
            {
                var medications = await _context.Medications.Where(m => medicationIds.Contains(m.MedicationId)).ToListAsync();
                medicalEvent.Medications = medications;
            }
            else
            {
                medicalEvent.Medications = new List<Medication>();
            }

            // Gán danh sách thiết bị y tế
            if (medicalSupplyIds != null && medicalSupplyIds.Count > 0)
            {
                var supplies = await _context.MedicalSupplies.Where(s => medicalSupplyIds.Contains(s.MedicalSupplyId)).ToListAsync();
                medicalEvent.MedicalSupplies = supplies;
            }
            else
            {
                medicalEvent.MedicalSupplies = new List<MedicalSupply>();
            }

            await _context.MedicalEvents.AddAsync(medicalEvent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds, List<int>? medicalSupplyIds)
        {
            var existing = await _context.MedicalEvents
                .Include(me => me.Medications)
                .Include(me => me.MedicalSupplies)
                .FirstOrDefaultAsync(me => me.MedicalEventId == medicalEvent.MedicalEventId);
            if (existing == null)
                return false;

            if (!string.IsNullOrWhiteSpace(medicalEvent.Type)) existing.Type = medicalEvent.Type.Trim();
            if (!string.IsNullOrWhiteSpace(medicalEvent.Description)) existing.Description = medicalEvent.Description.Trim();
            if (medicalEvent.Note != null) existing.Note = medicalEvent.Note.Trim();
            if (medicalEvent.Date != default) existing.Date = medicalEvent.Date;

            // Cập nhật danh sách thuốc nếu có truyền lên
            if (medicationIds != null)
            {
                var medications = await _context.Medications.Where(m => medicationIds.Contains(m.MedicationId)).ToListAsync();
                existing.Medications = medications;
            }

            // Cập nhật danh sách thiết bị y tế nếu có truyền lên
            if (medicalSupplyIds != null)
            {
                var supplies = await _context.MedicalSupplies.Where(s => medicalSupplyIds.Contains(s.MedicalSupplyId)).ToListAsync();
                existing.MedicalSupplies = supplies;
            }

            _context.MedicalEvents.Update(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMedicalEventAsync(int eventId)
        {
            var medicalEvent = await _context.MedicalEvents.FirstOrDefaultAsync(me => me.MedicalEventId == eventId);
            if (medicalEvent == null)
                return false;

            _context.MedicalEvents.Remove(medicalEvent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<(Student student, List<MedicalEvent> events)>> GetMedicalEventsByParentIdAsync(int parentId)
        {
            var students = await _context.Students
                .Where(s => s.ParentId == parentId)
                .Include(s => s.Parent)
                .ToListAsync();

            var result = new List<(Student, List<MedicalEvent>)>();
            foreach (var student in students)
            {
                var events = await _context.MedicalEvents
                    .Where(me => me.StudentId == student.StudentId)
                    .Include(me => me.Nurse)
                    .Include(me => me.Medications)
                    .Include(me => me.MedicalSupplies)
                    .AsNoTracking()
                    .ToListAsync();
                result.Add((student, events));
            }
            return result;
        }
    }
}