using BOs.Models;
using Repos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class VaccinationService : IVaccinationService
    {
        private readonly IVaccinationRepo _repo;
        public VaccinationService(IVaccinationRepo repo) { _repo = repo; }

        public Task<List<Vaccine>> GetAllVaccinesAsync() => _repo.GetAllVaccinesAsync();
        public Task<Vaccine?> GetVaccineByIdAsync(int id) => _repo.GetVaccineByIdAsync(id);
        public Task<Vaccine> CreateVaccineAsync(Vaccine vaccine) => _repo.CreateVaccineAsync(vaccine);

        public Task<List<VaccinationCampaign>> GetAllCampaignsAsync() => _repo.GetAllCampaignsAsync();
        public Task<VaccinationCampaign?> GetCampaignByIdAsync(int id) => _repo.GetCampaignByIdAsync(id);
        public Task<VaccinationCampaign> CreateCampaignAsync(VaccinationCampaign campaign) => _repo.CreateCampaignAsync(campaign);

        public Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId) => _repo.GetConsentsByCampaignAsync(campaignId);
        public Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId) => _repo.GetConsentsByParentIdAsync(parentId);
        public Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId) => _repo.GetConsentAsync(campaignId, studentId);
        public Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent) => _repo.CreateConsentAsync(consent);

        public Task<List<VaccinationRecord>> GetRecordsByCampaignAsync(int campaignId) => _repo.GetRecordsByCampaignAsync(campaignId);
        public Task<VaccinationRecord?> GetRecordByIdAsync(int id) => _repo.GetRecordByIdAsync(id);
        public Task<List<VaccinationRecord>> GetRecordsByStudentIdAsync(int studentId) => _repo.GetRecordsByStudentIdAsync(studentId);
        public Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record) => _repo.CreateRecordAsync(record);

        public Task<List<VaccinationFollowUp>> GetFollowUpsByRecordAsync(int recordId) => _repo.GetFollowUpsByRecordAsync(recordId);
        public Task<VaccinationFollowUp> CreateFollowUpAsync(VaccinationFollowUp followUp) => _repo.CreateFollowUpAsync(followUp);
    }
}