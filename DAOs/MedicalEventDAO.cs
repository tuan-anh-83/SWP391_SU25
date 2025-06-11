using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;

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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId)
        {
            return await _context.MedicalEvents
                .Include(me => me.Student)
                .Include(me => me.Nurse)
                .Include(me => me.Medications)
                .AsNoTracking()
                .FirstOrDefaultAsync(me => me.MedicalEventId == eventId);
        }

        public async Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent, List<int> medicationIds)
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

            await _context.MedicalEvents.AddAsync(medicalEvent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent, List<int>? medicationIds)
        {
            var existing = await _context.MedicalEvents
                .Include(me => me.Medications)
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
    }
}