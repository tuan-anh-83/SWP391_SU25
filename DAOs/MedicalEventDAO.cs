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
                .Include(me => me.MedicalEventMedications).ThenInclude(mem => mem.Medication)
                .Include(me => me.MedicalEventMedicalSupplies).ThenInclude(mes => mes.MedicalSupply)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MedicalEvent?> GetMedicalEventByIdAsync(int eventId)
        {
            return await _context.MedicalEvents
                .Include(me => me.Student)
                .Include(me => me.Nurse)
                .Include(me => me.MedicalEventMedications).ThenInclude(mem => mem.Medication)
                .Include(me => me.MedicalEventMedicalSupplies).ThenInclude(mes => mes.MedicalSupply)
                .AsNoTracking()
                .FirstOrDefaultAsync(me => me.MedicalEventId == eventId);
        }

        public async Task<bool> CreateMedicalEventAsync(
            MedicalEvent medicalEvent,
            List<MedicalEventMedication> medicationUsages,
            List<MedicalEventMedicalSupply> supplyUsages)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == medicalEvent.StudentId);
            var nurseExists = await _context.Accounts.AnyAsync(a => a.AccountID == medicalEvent.NurseId);
            if (!studentExists || !nurseExists)
                return false;

            await _context.MedicalEvents.AddAsync(medicalEvent);
            await _context.SaveChangesAsync();

            foreach (var mem in medicationUsages)
            {
                mem.MedicalEventId = medicalEvent.MedicalEventId;
                var medication = await _context.Medications.FindAsync(mem.MedicationId);
                if (medication == null || medication.Quantity < mem.QuantityUsed)
                    return false;
                medication.Quantity -= mem.QuantityUsed;
                await _context.MedicalEventMedications.AddAsync(mem);
            }

            foreach (var mes in supplyUsages)
            {
                mes.MedicalEventId = medicalEvent.MedicalEventId;
                var supply = await _context.MedicalSupplies.FindAsync(mes.MedicalSupplyId);
                if (supply == null || supply.Quantity < mes.QuantityUsed)
                    return false;
                supply.Quantity -= mes.QuantityUsed;
                await _context.MedicalEventMedicalSupplies.AddAsync(mes);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateMedicalEventAsync(
            MedicalEvent medicalEvent,
            List<MedicalEventMedication> medicationUsages,
            List<MedicalEventMedicalSupply> supplyUsages)
        {
            var existing = await _context.MedicalEvents
                .Include(me => me.MedicalEventMedications)
                .Include(me => me.MedicalEventMedicalSupplies)
                .FirstOrDefaultAsync(me => me.MedicalEventId == medicalEvent.MedicalEventId);

            if (existing == null)
                return false;

            if (!string.IsNullOrWhiteSpace(medicalEvent.Type)) existing.Type = medicalEvent.Type.Trim();
            if (!string.IsNullOrWhiteSpace(medicalEvent.Description)) existing.Description = medicalEvent.Description.Trim();
            if (medicalEvent.Note != null) existing.Note = medicalEvent.Note.Trim();
            if (medicalEvent.Date != default) existing.Date = medicalEvent.Date;

            // Trả lại số lượng thuốc/vật tư cũ
            foreach (var old in existing.MedicalEventMedications)
            {
                var medication = await _context.Medications.FindAsync(old.MedicationId);
                if (medication != null)
                    medication.Quantity += old.QuantityUsed;
            }
            _context.MedicalEventMedications.RemoveRange(existing.MedicalEventMedications);

            foreach (var old in existing.MedicalEventMedicalSupplies)
            {
                var supply = await _context.MedicalSupplies.FindAsync(old.MedicalSupplyId);
                if (supply != null)
                    supply.Quantity += old.QuantityUsed;
            }
            _context.MedicalEventMedicalSupplies.RemoveRange(existing.MedicalEventMedicalSupplies);

            // Thêm bản ghi mới và trừ số lượng
            foreach (var mem in medicationUsages)
            {
                mem.MedicalEventId = existing.MedicalEventId;
                var medication = await _context.Medications.FindAsync(mem.MedicationId);
                if (medication == null || medication.Quantity < mem.QuantityUsed)
                    return false;
                medication.Quantity -= mem.QuantityUsed;
                await _context.MedicalEventMedications.AddAsync(mem);
            }

            foreach (var mes in supplyUsages)
            {
                mes.MedicalEventId = existing.MedicalEventId;
                var supply = await _context.MedicalSupplies.FindAsync(mes.MedicalSupplyId);
                if (supply == null || supply.Quantity < mes.QuantityUsed)
                    return false;
                supply.Quantity -= mes.QuantityUsed;
                await _context.MedicalEventMedicalSupplies.AddAsync(mes);
            }

            _context.MedicalEvents.Update(existing);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMedicalEventAsync(int eventId)
        {
            var existing = await _context.MedicalEvents
                .Include(me => me.MedicalEventMedications)
                .Include(me => me.MedicalEventMedicalSupplies)
                .FirstOrDefaultAsync(me => me.MedicalEventId == eventId);

            if (existing == null)
                return false;

            foreach (var mem in existing.MedicalEventMedications)
            {
                var medication = await _context.Medications.FindAsync(mem.MedicationId);
                if (medication != null)
                    medication.Quantity += mem.QuantityUsed;
            }
            foreach (var mes in existing.MedicalEventMedicalSupplies)
            {
                var supply = await _context.MedicalSupplies.FindAsync(mes.MedicalSupplyId);
                if (supply != null)
                    supply.Quantity += mes.QuantityUsed;
            }

            _context.MedicalEventMedications.RemoveRange(existing.MedicalEventMedications);
            _context.MedicalEventMedicalSupplies.RemoveRange(existing.MedicalEventMedicalSupplies);
            _context.MedicalEvents.Remove(existing);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<(Student student, List<MedicalEvent> events)>> GetMedicalEventsByParentIdAsync(int parentId)
        {
            var students = await _context.Students
                .Where(s => s.ParentId == parentId)
                .Include(s => s.Parent)
                .Include(s => s.Class)
                .ToListAsync();

            var result = new List<(Student, List<MedicalEvent>)>();
            foreach (var student in students)
            {
                var events = await _context.MedicalEvents
                    .Where(me => me.StudentId == student.StudentId)
                    .Include(me => me.Nurse)
                    .Include(me => me.MedicalEventMedications).ThenInclude(mem => mem.Medication)
                    .Include(me => me.MedicalEventMedicalSupplies).ThenInclude(mes => mes.MedicalSupply)
                    .AsNoTracking()
                    .ToListAsync();
                result.Add((student, events));
            }
            return result;
        }
    }
}