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
        private static readonly object lockObj = new object();

        private VaccinationDAO() { }

        public static VaccinationDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new VaccinationDAO();
                        }
                    }
                }
                return instance;
            }
        }

        // Vaccine
        public async Task<List<Vaccine>> GetAllVaccinesAsync()
        {
            using var context = new DataContext();
            return await context.Vaccines.ToListAsync();
        }

        public async Task<Vaccine?> GetVaccineByIdAsync(int id)
        {
            using var context = new DataContext();
            return await context.Vaccines.FindAsync(id);
        }

        public async Task<Vaccine> CreateVaccineAsync(Vaccine vaccine)
        {
            using var context = new DataContext();
            await context.Vaccines.AddAsync(vaccine);
            await context.SaveChangesAsync();
            return vaccine;
        }

        // VaccinationCampaign
        public async Task<List<VaccinationCampaign>> GetAllCampaignsAsync()
        {
            using var context = new DataContext();
            return await context.VaccinationCampaigns.Include(c => c.Vaccine).ToListAsync();
        }

        public async Task<VaccinationCampaign?> GetCampaignByIdAsync(int id)
        {
            using var context = new DataContext();
            return await context.VaccinationCampaigns
                .Include(c => c.Vaccine)
                .FirstOrDefaultAsync(c => c.CampaignId == id);
        }

        public async Task<VaccinationCampaign> CreateCampaignAsync(VaccinationCampaign campaign)
        {
            using var context = new DataContext();
            await context.VaccinationCampaigns.AddAsync(campaign);
            await context.SaveChangesAsync();
            return campaign;
        }

        public async Task<bool> CampaignNameExistsAsync(string name)
        {
            using var context = new DataContext();
            return await context.VaccinationCampaigns.AnyAsync(c => c.Name == name);
        }

        public async Task<bool> CampaignTimeConflictAsync(DateTime date)
        {
            using var context = new DataContext();
            return await context.VaccinationCampaigns.AnyAsync(c => c.Date == date);
        }

        // VaccinationConsent
        public async Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId)
        {
            using var context = new DataContext();
            return await context.VaccinationConsents
                .Include(c => c.Student)
                .ThenInclude(c => c.Class)
                .Include(c => c.Parent)
                .Where(c => c.CampaignId == campaignId)
                .ToListAsync();
        }

        public async Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId)
        {
            using var context = new DataContext();
            return await context.VaccinationConsents
                .Include(c => c.Campaign)
                .Include(c => c.Student)
                .Where(c => c.ParentId == parentId)
                .ToListAsync();
        }

        /*public async Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId)
        {
            using var context = new DataContext();
            return await context.VaccinationConsents
                .FirstOrDefaultAsync(c => c.CampaignId == campaignId && c.StudentId == studentId);
        }*/

        public async Task<VaccinationConsent> GetConsentAsync(int campaignId, int studentId, int parentId)
        {
            using var context = new DataContext();
            return await context.VaccinationConsents
                .Where(c => c.CampaignId == campaignId && c.StudentId == studentId && c.ParentId == parentId)
                .OrderByDescending(c => c.DateConfirmed)
                .FirstOrDefaultAsync();
        }

        public async Task<VaccinationConsent> GetLatestConsentAsync(int campaignId, int studentId)
        {
            using var context = new DataContext();
            return await context.VaccinationConsents
                .Where(c => c.CampaignId == campaignId && c.StudentId == studentId)
                .OrderByDescending(c => c.DateConfirmed)
                .FirstOrDefaultAsync();
        }

        public async Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent)
        {
            using var context = new DataContext();
            await context.VaccinationConsents.AddAsync(consent);
            await context.SaveChangesAsync();
            return consent;
        }

        public async Task<VaccinationConsent> UpdateConsentAsync(VaccinationConsent consent)
        {
            using var context = new DataContext();
            context.VaccinationConsents.Update(consent);
            await context.SaveChangesAsync();
            return consent;
        }

        public async Task AutoRejectUnconfirmedConsentsAsync(int campaignId, DateTime campaignDate)
        {
            using var context = new DataContext();
            var unconfirmedConsents = await context.VaccinationConsents
                .Where(c => c.CampaignId == campaignId && c.IsAgreed == null)
                .ToListAsync();

            foreach (var consent in unconfirmedConsents)
            {
                consent.IsAgreed = false;
                consent.DateConfirmed = campaignDate;
            }
            await context.SaveChangesAsync();
        }

        // VaccinationRecord
        public async Task<List<VaccinationRecord>> GetRecordsByCampaignAsync(int campaignId)
        {
            using var context = new DataContext();
            return await context.VaccinationRecords
                .Include(r => r.Student)
                .Include(r => r.Nurse)
                .Where(r => r.CampaignId == campaignId)
                .ToListAsync();
        }

        public async Task<VaccinationRecord?> GetRecordByIdAsync(int id)
        {
            using var context = new DataContext();
            return await context.VaccinationRecords
                .Include(r => r.Student)
                .Include(r => r.Nurse)
                .FirstOrDefaultAsync(r => r.RecordId == id);
        }

        public async Task<List<VaccinationRecord>> GetRecordsByStudentIdAsync(int studentId)
        {
            using var context = new DataContext();
            return await context.VaccinationRecords
                .Include(r => r.Campaign)
                .Where(r => r.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record)
        {
            using var context = new DataContext();
            await context.VaccinationRecords.AddAsync(record);
            await context.SaveChangesAsync();
            return record;
        }

        // VaccinationFollowUp
        public async Task<List<VaccinationFollowUp>> GetFollowUpsByRecordAsync(int recordId)
        {
            using var context = new DataContext();
            return await context.VaccinationFollowUps
                .Where(f => f.RecordId == recordId)
                .ToListAsync();
        }

        public async Task<VaccinationFollowUp> CreateFollowUpAsync(VaccinationFollowUp followUp)
        {
            using var context = new DataContext();
            await context.VaccinationFollowUps.AddAsync(followUp);
            await context.SaveChangesAsync();
            return followUp;
        }
    }
}
