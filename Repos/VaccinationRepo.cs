using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class VaccinationRepo : IVaccinationRepo
    {
        // Vaccine
        public Task<List<Vaccine>> GetAllVaccinesAsync() => VaccinationDAO.Instance.GetAllVaccinesAsync();
        public Task<Vaccine?> GetVaccineByIdAsync(int id) => VaccinationDAO.Instance.GetVaccineByIdAsync(id);
        public Task<Vaccine> CreateVaccineAsync(Vaccine vaccine) => VaccinationDAO.Instance.CreateVaccineAsync(vaccine);

        // Campaign
        public Task<List<VaccinationCampaign>> GetAllCampaignsAsync() => VaccinationDAO.Instance.GetAllCampaignsAsync();
        public Task<VaccinationCampaign?> GetCampaignByIdAsync(int id) => VaccinationDAO.Instance.GetCampaignByIdAsync(id);
        public Task<VaccinationCampaign> CreateCampaignAsync(VaccinationCampaign campaign) => VaccinationDAO.Instance.CreateCampaignAsync(campaign);
        public Task<bool> CampaignNameExistsAsync(string name) => VaccinationDAO.Instance.CampaignNameExistsAsync(name);
        public Task<bool> CampaignTimeConflictAsync(DateTime date) => VaccinationDAO.Instance.CampaignTimeConflictAsync(date);

        // Consent
        public Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId) => VaccinationDAO.Instance.GetConsentsByCampaignAsync(campaignId);
        public Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId) => VaccinationDAO.Instance.GetConsentsByParentIdAsync(parentId);
        public Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId, int parentId) => VaccinationDAO.Instance.GetConsentAsync(campaignId, studentId, parentId);
        public Task<VaccinationConsent?> GetLatestConsentAsync(int campaignId, int studentId) => VaccinationDAO.Instance.GetLatestConsentAsync(campaignId, studentId);
        public Task<VaccinationConsent> UpdateConsentAsync(VaccinationConsent consent) => VaccinationDAO.Instance.UpdateConsentAsync(consent);
        public Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent) => VaccinationDAO.Instance.CreateConsentAsync(consent);
        public Task AutoRejectUnconfirmedConsentsAsync(int campaignId, DateTime campaignDate)
            => VaccinationDAO.Instance.AutoRejectUnconfirmedConsentsAsync(campaignId, campaignDate);

        // Record
        public Task<List<VaccinationRecord>> GetRecordsByCampaignAsync(int campaignId) => VaccinationDAO.Instance.GetRecordsByCampaignAsync(campaignId);
        public Task<VaccinationRecord?> GetRecordByIdAsync(int id) => VaccinationDAO.Instance.GetRecordByIdAsync(id);
        public Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record) => VaccinationDAO.Instance.CreateRecordAsync(record);
        public Task<List<VaccinationRecord>> GetRecordsByStudentIdAsync(int studentId) => VaccinationDAO.Instance.GetRecordsByStudentIdAsync(studentId);

        // FollowUp
        public Task<List<VaccinationFollowUp>> GetFollowUpsByRecordAsync(int recordId) => VaccinationDAO.Instance.GetFollowUpsByRecordAsync(recordId);
        public Task<VaccinationFollowUp> CreateFollowUpAsync(VaccinationFollowUp followUp) => VaccinationDAO.Instance.CreateFollowUpAsync(followUp);
    }
}