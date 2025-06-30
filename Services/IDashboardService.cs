using System.Security.Claims;
using static DAOs.DashboardDAO;


namespace Services
{
    public interface IDashboardService
    {
        Task<Dictionary<string, int>> GetOverviewAsync(int accountId);
        Task<Dictionary<string, List<TrendDataPoint>>> GetTrendsAsync(int accountId, string period);
        Task<List<RecentActivityData>> GetRecentActivitiesAsync(int accountId, int page, int pageSize, string? type);

        Task<(Dictionary<string, int> byGender, Dictionary<string, int> byAge, List<(string className, int count)>
            byClass)> GetStudentDistributionAsync(int accountId);

        Task<(List<(string month, double value, int count)> averageHeight, List<(string month, double value, int count)>
            averageWeight)> GetGrowthTrendsAsync(int accountId, string period, string ageGroup);
        Task<(List<(string Date, int Accidents, int Illnesses, int Fevers, int Others)> Timeline, Dictionary<string, int> TotalsByType)>
            GetMedicalEventsTimelineAsync(int accountId, string eventType, string period);
        Task<List<(string Condition, int Count, double Percentage)>> GetCommonConditionsAsync(int accountId, string period);
        Task<(List<(string Name, int Used, int Remaining)> MostUsedSupplies, List<(string Name, int Current, int Minimum)>
            LowStockAlerts, List<(string Name, string ExpiryDate, int Quantity)> ExpiringItems)> GetSupplyUsageAsync(int accountId, string period);
        // TAB 4: Vaccination Analytics
        Task<(double OverallCoverage, List<(string CampaignName, int TargetCount, int CompletedCount, double CoverageRate, double ConsentRate)>
            ByCampaign, List<(string ClassName, double Coverage)> ByClass)> GetVaccinationCoverageAsync(int accountId, int? campaignId, int? classId);
        Task<(List<(string Date, int Planned, int Actual)> Timeline, (int TotalPlanned, int TotalCompleted, double CompletionRate,
            double AveragePerDay, int DaysToComplete) Summary)> GetCampaignEffectivenessAsync(int accountId, int? campaignId);

        // TAB 5: Activity Analytics
        Task<(int TotalBookings, int Completed, int Cancelled, double CompletionRate, List<(string Reason, int Count)>
            ByReason, List<(string TimeSlot, int Bookings)> ByTimeSlot)> GetConsultationStatisticsAsync(int accountId, string period);
        Task<(int TotalRequests, int Approved, int Rejected, int Pending, double ApprovalRate, List<(string Medication, int Requests)>
            CommonMedications)> GetMedicationRequestsAsync(int accountId, string period);

        Task<(int TotalNurses, List<(string NurseName, int HealthChecks, int MedicalEvents, int Consultations, int
                WorkingDays,
                double AveragePerDay)> ByNurse, (int HealthChecks, int MedicalEvents, int Consultations, int
            MedicationApprovals) WorkloadDistribution)> GetNurseActivityAsync(int accountId, int? nurseId,
            string period);
        // TAB 6: Reports & Trends
        Task<HealthSummaryData> GetHealthSummaryAsync(int accountId, string period);
        Task<ComparisonData> GetComparisonAsync(int accountId, string metric, string period1, string period2);
    }
}