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
                .Include(me => me.Medication)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId)
        {
            return await _context.MedicalEvents
                .Include(me => me.Student)
                .Include(me => me.Nurse)
                .Include(me => me.Medication)
                .AsNoTracking()
                .FirstOrDefaultAsync(me => me.MedicalEventId == eventId);
        }

        public async Task<bool> CreateMedicalEventAsync(MedicalEvent medicalEvent)
        {
            if (string.IsNullOrWhiteSpace(medicalEvent.Type) || string.IsNullOrWhiteSpace(medicalEvent.Description))
                return false;

            // Kiểm tra StudentId, NurseId, và MedicationId (nếu có)
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == medicalEvent.StudentId);
            var nurseExists = await _context.Accounts.AnyAsync(a => a.AccountID == medicalEvent.NurseId);
            if (!studentExists || !nurseExists)
                return false;

            if (medicalEvent.MedicationId.HasValue)
            {
                var medicationExists = await _context.Medications.AnyAsync(m => m.MedicationId == medicalEvent.MedicationId);
                if (!medicationExists)
                    return false;
            }

            medicalEvent.Type = medicalEvent.Type.Trim();
            medicalEvent.Description = medicalEvent.Description.Trim();
            medicalEvent.Note = medicalEvent.Note?.Trim();

            await _context.MedicalEvents.AddAsync(medicalEvent);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateMedicalEventAsync(MedicalEvent medicalEvent)
        {
            var existing = await _context.MedicalEvents.FirstOrDefaultAsync(me => me.MedicalEventId == medicalEvent.MedicalEventId);
            if (existing == null)
                return false;

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(medicalEvent.Type) && medicalEvent.Type.Trim() != existing.Type)
            {
                existing.Type = medicalEvent.Type.Trim();
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(medicalEvent.Description) && medicalEvent.Description.Trim() != existing.Description)
            {
                existing.Description = medicalEvent.Description.Trim();
                hasChanges = true;
            }

            if (medicalEvent.Note != null && medicalEvent.Note.Trim() != existing.Note)
            {
                existing.Note = medicalEvent.Note.Trim();
                hasChanges = true;
            }

            if (medicalEvent.Date != default && medicalEvent.Date != existing.Date)
            {
                existing.Date = medicalEvent.Date;
                hasChanges = true;
            }

            if (medicalEvent.MedicationId != existing.MedicationId)
            {
                if (medicalEvent.MedicationId.HasValue)
                {
                    var medicationExists = await _context.Medications.AnyAsync(m => m.MedicationId == medicalEvent.MedicationId);
                    if (!medicationExists)
                        return false;
                }
                existing.MedicationId = medicalEvent.MedicationId;
                hasChanges = true;
            }

            if (hasChanges)
            {
                _context.MedicalEvents.Update(existing);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
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