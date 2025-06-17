using BOs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IVaccinationService
    {
        // Vaccine
        Task<List<Vaccine>> GetAllVaccinesAsync();
        Task<Vaccine?> GetVaccineByIdAsync(int id);
        Task<Vaccine> CreateVaccineAsync(Vaccine vaccine);

        // Campaign
        Task<List<VaccinationCampaign>> GetAllCampaignsAsync();
        Task<VaccinationCampaign?> GetCampaignByIdAsync(int id);
        Task<VaccinationCampaign> CreateCampaignAsync(VaccinationCampaign campaign);

        // Consent
        Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId);
        Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId);
        Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId);
        Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent);

        // Record
        Task<List<VaccinationRecord>> GetRecordsByCampaignAsync(int campaignId);
        Task<VaccinationRecord?> GetRecordByIdAsync(int id);
        Task<VaccinationRecord> CreateRecordAsync(VaccinationRecord record);
        Task<List<VaccinationRecord>> GetRecordsByStudentIdAsync(int studentId);

        // FollowUp
        Task<List<VaccinationFollowUp>> GetFollowUpsByRecordAsync(int recordId);
        Task<VaccinationFollowUp> CreateFollowUpAsync(VaccinationFollowUp followUp);
    }
}