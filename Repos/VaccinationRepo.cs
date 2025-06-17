using BOs.Models;
using DAOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public class VaccinationRepo : IVaccinationRepo
    {
        public Task<List<Vaccine>> GetAllVaccinesAsync() => VaccinationDAO.Instance.GetAllVaccinesAsync();
        public Task<Vaccine?> GetVaccineByIdAsync(int id) => VaccinationDAO.Instance.GetVaccineByIdAsync(id);
        public Task<Vaccine> CreateVaccineAsync(Vaccine vaccine) => VaccinationDAO.Instance.CreateVaccineAsync(vaccine);

        public Task<List<VaccinationCampaign>> GetAllCampaignsAsync() => VaccinationDAO.Instance.GetAllCampaignsAsync();
        public Task<VaccinationCampaign?> GetCampaignByIdAsync(int id) => VaccinationDAO.Instance.GetCampaignByIdAsync(id);
        public Task<VaccinationCampaign> CreateCampaignAsync(VaccinationCampaign campaign) => VaccinationDAO.Instance.CreateCampaignAsync(campaign);

        public Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId) => VaccinationDAO.Instance.GetConsentsByCampaignAsync(campaignId);
        public Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId) => VaccinationDAO.Instance.GetConsentsByParentIdAsync(parentId);
        public Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId) => VaccinationDAO.Instance.GetConsentAsync(campaignId, studentId);
        public Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent) => VaccinationDAO.Instance.CreateConsentAsync(consent);

        public Task<List<VaccinationRecord>> GetRecordsByCampaignAsync(int campaignId) => VaccinationDAO.Instance.GetRecordsByCampaignAsync(campaignId);
        public Task<VaccinationRecord?> GetRecordByIdAsync(int id) => VaccinationDAO.Instance.GetRecordByIdAsync(id);
        public Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record) => VaccinationDAO.Instance.CreateRecordAsync(record);
        public Task<List<VaccinationRecord>> GetRecordsByStudentIdAsync(int studentId) => VaccinationDAO.Instance.GetRecordsByStudentIdAsync(studentId);

        public Task<List<VaccinationFollowUp>> GetFollowUpsByRecordAsync(int recordId) => VaccinationDAO.Instance.GetFollowUpsByRecordAsync(recordId);
        public Task<VaccinationFollowUp> CreateFollowUpAsync(VaccinationFollowUp followUp) => VaccinationDAO.Instance.CreateFollowUpAsync(followUp);
    }
}