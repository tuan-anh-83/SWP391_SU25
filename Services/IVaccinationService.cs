using BOs.Models;
using System;
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
        Task<bool> CampaignNameExistsAsync(string name);
        Task<bool> CampaignTimeConflictAsync(DateTime date);

        // Consent
        Task<List<VaccinationConsent>> GetConsentsByCampaignAsync(int campaignId);
        Task<List<VaccinationConsent>> GetConsentsByParentIdAsync(int parentId);
        Task<VaccinationConsent?> GetConsentAsync(int campaignId, int studentId, int parentId);
        Task<VaccinationConsent?> GetLatestConsentAsync(int campaignId, int studentId);
        Task<VaccinationConsent> UpdateConsentAsync(VaccinationConsent consent);
        Task<VaccinationConsent> CreateConsentAsync(VaccinationConsent consent);
        Task AutoRejectUnconfirmedConsentsAsync(int campaignId, DateTime campaignDate);

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