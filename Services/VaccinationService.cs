using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class VaccinationService : IVaccinationService
    {
        private readonly VaccinationDAO _dao = VaccinationDAO.Instance;

        public async Task<List<Vaccine>> GetAllVaccinesAsync() => await _dao.GetAllVaccinesAsync();
        public async Task<Vaccine?> GetVaccineByIdAsync(int id) => await _dao.GetVaccineByIdAsync(id);
        public async Task<Vaccine> CreateVaccineAsync(Vaccine vaccine) => await _dao.CreateVaccineAsync(vaccine);

        public async Task<List<VaccinationCampaign>> GetAllCampaignsAsync() => await _dao.GetAllCampaignsAsync();
        public async Task<VaccinationCampaign?> GetCampaignByIdAsync(int id) => await _dao.GetCampaignByIdAsync(id);
        public async Task<VaccinationCampaign> CreateCampaignAsync(VaccinationCampaign campaign) => await _dao.CreateCampaignAsync(campaign);

        public async Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId) => await _dao.GetConsentsByCampaignAsync(campaignId);
        public async Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId) => await _dao.GetConsentsByParentIdAsync(parentId);
        public async Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId, int parentId) => await _dao.GetConsentAsync(campaignId, studentId, parentId);
        public async Task<VaccinationConsent?> GetLatestConsentAsync(int campaignId, int studentId) => await _dao.GetLatestConsentAsync(campaignId, studentId);
        public async Task<VaccinationConsent> UpdateConsentAsync(VaccinationConsent consent) => await _dao.UpdateConsentAsync(consent);
        public async Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent) => await _dao.CreateConsentAsync(consent);

        public async Task<List<VaccinationRecord>> GetRecordsByCampaignAsync(int campaignId) => await _dao.GetRecordsByCampaignAsync(campaignId);
        public async Task<VaccinationRecord?> GetRecordByIdAsync(int id) => await _dao.GetRecordByIdAsync(id);
        public async Task<List<VaccinationRecord>> GetRecordsByStudentIdAsync(int studentId) => await _dao.GetRecordsByStudentIdAsync(studentId);
        public async Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record) => await _dao.CreateRecordAsync(record);

        public async Task<List<VaccinationFollowUp>> GetFollowUpsByRecordAsync(int recordId) => await _dao.GetFollowUpsByRecordAsync(recordId);
        public async Task<VaccinationFollowUp> CreateFollowUpAsync(VaccinationFollowUp followUp) => await _dao.CreateFollowUpAsync(followUp);

        public Task<bool> CampaignNameExistsAsync(string name)
            => _dao.CampaignNameExistsAsync(name);

        public Task<bool> CampaignTimeConflictAsync(DateTime date)
            => _dao.CampaignTimeConflictAsync(date);

        public Task AutoRejectUnconfirmedConsentsAsync(int campaignId, DateTime campaignDate)
            => _dao.AutoRejectUnconfirmedConsentsAsync(campaignId, campaignDate);
    }
}