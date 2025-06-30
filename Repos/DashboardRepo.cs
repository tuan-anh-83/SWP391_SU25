using DAOs;
using static DAOs.DashboardDAO;

namespace Repos
{
    public class DashboardRepo : IDashboardRepo
    {
        private readonly DashboardDAO _dao = DashboardDAO.Instance;

        public Task<Dictionary<string, int>> GetDashboardOverviewAsync() => _dao.GetDashboardOverviewAsync();
        public Task<Dictionary<string, List<TrendDataPoint>>> GetDashboardTrendsAsync(string period) => _dao.GetDashboardTrendsAsync(period);
        public Task<List<RecentActivityData>> GetRecentActivitiesAsync(int page, int pageSize, string? type) => _dao.GetRecentActivitiesAsync(page, pageSize, type);
        public Task<(Dictionary<string, int> byGender, Dictionary<string, int> byAge, List<(string className, int count)> byClass)> GetStudentDistributionAsync()
            => _dao.GetStudentDistributionAsync();
        public Task<(List<(string month, double value, int count)> averageHeight, List<(string month, double value, int count)> averageWeight)> GetGrowthTrendsAsync(string period, string ageGroup)
            => _dao.GetGrowthTrendsAsync(period, ageGroup);
        public Task<(List<(string Date, int Accidents, int Illnesses, int Fevers, int Others)> Timeline, Dictionary<string, int> TotalsByType)>
            GetMedicalEventsTimelineAsync(string eventType, string period) => _dao.GetMedicalEventsTimelineAsync(eventType, period);

        public Task<List<(string Condition, int Count, double Percentage)>> GetCommonConditionsAsync(string period)
            => _dao.GetCommonConditionsAsync(period);

        public Task<(List<(string Name, int Used, int Remaining)> MostUsedSupplies, List<(string Name, int Current, int Minimum)>
            LowStockAlerts, List<(string Name, string ExpiryDate, int Quantity)> ExpiringItems)> GetSupplyUsageAsync(string period) => _dao.GetSupplyUsageAsync(period);
        public Task<(double OverallCoverage, List<(string CampaignName, int TargetCount, int CompletedCount, double CoverageRate, double ConsentRate)> ByCampaign, List<(string ClassName, double Coverage)> ByClass)> GetVaccinationCoverageAsync(int? campaignId, int? classId)
            => _dao.GetVaccinationCoverageAsync(campaignId, classId);

        public Task<(List<(string Date, int Planned, int Actual)> Timeline, (int TotalPlanned, int TotalCompleted, double CompletionRate, double AveragePerDay, int DaysToComplete) Summary)> GetCampaignEffectivenessAsync(int? campaignId)
            => _dao.GetCampaignEffectivenessAsync(campaignId);

        public Task<(int TotalBookings, int Completed, int Cancelled, double CompletionRate, List<(string Reason,
            int Count)> ByReason, List<(string TimeSlot, int Bookings)> ByTimeSlot)> GetConsultationStatisticsAsync(string period)
            => _dao.GetConsultationStatisticsAsync(period);

        public Task<(int TotalRequests, int Approved, int Rejected, int Pending, double ApprovalRate,
            List<(string Medication, int Requests)> CommonMedications)> GetMedicationRequestsAsync(string period)
            => _dao.GetMedicationRequestsAsync(period);

        public Task<(int TotalNurses, List<(string NurseName, int HealthChecks, int MedicalEvents,
            int Consultations, int WorkingDays, double AveragePerDay)> ByNurse, (int HealthChecks, int MedicalEvents, int Consultations, int MedicationApprovals) WorkloadDistribution)> GetNurseActivityAsync(int? nurseId, string period)
            => _dao.GetNurseActivityAsync(nurseId, period);
        public Task<HealthSummaryData> GetHealthSummaryAsync(string period)
            => _dao.GetHealthSummaryAsync(period);

        public Task<ComparisonData> GetComparisonAsync(string metric, string period1, string period2)
            => _dao.GetComparisonAsync(metric, period1, period2);
    }
}