using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class VaccinationDAO
    {
        private static VaccinationDAO instance = null;
        private readonly DataContext _context;

        private VaccinationDAO()
        {
            _context = new DataContext();
        }

        public static VaccinationDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VaccinationDAO();
                }
                return instance;
            }
        }

        // Vaccine
        public async Task<List<Vaccine>> GetAllVaccinesAsync() => await _context.Vaccines.ToListAsync();
        public async Task<Vaccine?> GetVaccineByIdAsync(int id) => await _context.Vaccines.FindAsync(id);
        public async Task<Vaccine> CreateVaccineAsync(Vaccine vaccine)
        {
            await _context.Vaccines.AddAsync(vaccine);
            await _context.SaveChangesAsync();
            return vaccine;
        }

        // VaccinationCampaign
        public async Task<List<VaccinationCampaign>> GetAllCampaignsAsync() =>
            await _context.VaccinationCampaigns.Include(c => c.Vaccine).ToListAsync();
        public async Task<VaccinationCampaign?> GetCampaignByIdAsync(int id) =>
            await _context.VaccinationCampaigns.Include(c => c.Vaccine).FirstOrDefaultAsync(c => c.CampaignId == id);
        public async Task<VaccinationCampaign> CreateCampaignAsync(VaccinationCampaign campaign)
        {
            await _context.VaccinationCampaigns.AddAsync(campaign);
            await _context.SaveChangesAsync();
            return campaign;
        }

        // VaccinationConsent
        public async Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId) =>
            await _context.VaccinationConsents
                .Include(c => c.Student)
                .Include(c => c.Parent)
                .Where(c => c.CampaignId == campaignId)
                .ToListAsync();

        public async Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId) =>
            await _context.VaccinationConsents
                .Include(c => c.Campaign)
                .Include(c => c.Student)
                .Where(c => c.ParentId == parentId)
                .ToListAsync();

        public async Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId) =>
            await _context.VaccinationConsents
                .FirstOrDefaultAsync(c => c.CampaignId == campaignId && c.StudentId == studentId);

        public async Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent)
        {
            await _context.VaccinationConsents.AddAsync(consent);
            await _context.SaveChangesAsync();
            return consent;
        }

        // VaccinationRecord
        public async Task<List<VaccinationRecord>> GetRecordsByCampaignAsync(int campaignId) =>
            await _context.VaccinationRecords
                .Include(r => r.Student)
                .Include(r => r.Nurse)
                .Where(r => r.CampaignId == campaignId)
                .ToListAsync();

        public async Task<VaccinationRecord?> GetRecordByIdAsync(int id) =>
            await _context.VaccinationRecords
                .Include(r => r.Student)
                .Include(r => r.Nurse)
                .FirstOrDefaultAsync(r => r.RecordId == id);

        public async Task<List<VaccinationRecord>> GetRecordsByStudentIdAsync(int studentId)
        {
            return await _context.VaccinationRecords
                .Include(r => r.Campaign)
                .Where(r => r.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record)
        {
            await _context.VaccinationRecords.AddAsync(record);
            await _context.SaveChangesAsync();
            return record;
        }

        // VaccinationFollowUp
        public async Task<List<VaccinationFollowUp>> GetFollowUpsByRecordAsync(int recordId) =>
            await _context.VaccinationFollowUps.Where(f => f.RecordId == recordId).ToListAsync();

        public async Task<VaccinationFollowUp> CreateFollowUpAsync(VaccinationFollowUp followUp)
        {
            await _context.VaccinationFollowUps.AddAsync(followUp);
            await _context.SaveChangesAsync();
            return followUp;
        }
    }
}