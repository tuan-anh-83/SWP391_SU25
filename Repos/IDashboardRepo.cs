using System.Security.Claims;
using static DAOs.DashboardDAO;

namespace Repos
{
    public interface IDashboardRepo
    {
        Task<Dictionary<string, int>> GetDashboardOverviewAsync();
        Task<Dictionary<string, List<TrendDataPoint>>> GetDashboardTrendsAsync(string period);
        Task<List<RecentActivityData>> GetRecentActivitiesAsync(int page, int pageSize, string? type);
        Task<(Dictionary<string, int> byGender, Dictionary<string, int> byAge, List<(string className, int count)> byClass)> GetStudentDistributionAsync();
        Task<(List<(string month, double value, int count)> averageHeight, List<(string month, double value, int count)> averageWeight)>
            GetGrowthTrendsAsync(string period, string ageGroup);
        Task<(List<(string Date, int Accidents, int Illnesses, int Fevers, int Others)> Timeline, Dictionary<string, int> TotalsByType)>
            GetMedicalEventsTimelineAsync(string eventType, string period);
        Task<List<(string Condition, int Count, double Percentage)>> GetCommonConditionsAsync(string period);
        Task<(List<(string Name, int Used, int Remaining)> MostUsedSupplies, List<(string Name, int Current, int Minimum)>
            LowStockAlerts, List<(string Name, string ExpiryDate, int Quantity)> ExpiringItems)> GetSupplyUsageAsync(string period);
        // TAB 4: Vaccination Analytics
        Task<(double OverallCoverage, List<(string CampaignName, int TargetCount, int CompletedCount, double CoverageRate, double ConsentRate)>
            ByCampaign, List<(string ClassName, double Coverage)> ByClass)> GetVaccinationCoverageAsync(int? campaignId, int? classId);
        Task<(List<(string Date, int Planned, int Actual)> Timeline, (int TotalPlanned, int TotalCompleted, double CompletionRate,
            double AveragePerDay, int DaysToComplete) Summary)> GetCampaignEffectivenessAsync(int? campaignId);
        // TAB 5: Activity Analytics
        Task<(int TotalBookings, int Completed, int Cancelled, double CompletionRate, List<(string Reason, int Count)>
            ByReason, List<(string TimeSlot, int Bookings)> ByTimeSlot)> GetConsultationStatisticsAsync(string period);
        Task<(int TotalRequests, int Approved, int Rejected, int Pending, double ApprovalRate, List<(string Medication,
            int Requests)> CommonMedications)> GetMedicationRequestsAsync(string period);
        Task<(int TotalNurses, List<(string NurseName, int HealthChecks, int MedicalEvents, int Consultations,
            int WorkingDays, double AveragePerDay)> ByNurse, (int HealthChecks, int MedicalEvents, int Consultations,
            int MedicationApprovals) WorkloadDistribution)> GetNurseActivityAsync(int? nurseId, string period);
        // TAB 6: Reports & Trends
        Task<HealthSummaryData> GetHealthSummaryAsync(string period);
        Task<ComparisonData> GetComparisonAsync(string metric, string period1, string period2);
    }
}