using DAOs;
using Repos;
using static DAOs.DashboardDAO;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepo _dashboardRepo;
        private readonly IAccountService _accountService;

        public DashboardService(IDashboardRepo dashboardRepo, IAccountService accountService)
        {
            _dashboardRepo = dashboardRepo;
            _accountService = accountService;
        }

        public async Task<Dictionary<string, int>> GetOverviewAsync(int accountId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetDashboardOverviewAsync();
        }

        public async Task<Dictionary<string, List<TrendDataPoint>>> GetTrendsAsync(int accountId, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetDashboardTrendsAsync(period);
        }

        public async Task<List<RecentActivityData>> GetRecentActivitiesAsync(int accountId, int page, int pageSize, string? type)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetRecentActivitiesAsync(page, pageSize, type);
        }
        public async Task<(Dictionary<string, int> byGender, Dictionary<string, int> byAge, List<(string className, int count)> byClass)> GetStudentDistributionAsync(int accountId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }
            return await _dashboardRepo.GetStudentDistributionAsync();
        }

        public async Task<(List<(string month, double value, int count)> averageHeight, List<(string month, double value, int count)> averageWeight)> GetGrowthTrendsAsync(int accountId, string period, string ageGroup)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }
            return await _dashboardRepo.GetGrowthTrendsAsync(period, ageGroup);
        }
        public async Task<(List<(string Date, int Accidents, int Illnesses, int Fevers, int Others)> Timeline, Dictionary<string, int> TotalsByType)> GetMedicalEventsTimelineAsync(int accountId, string eventType, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetMedicalEventsTimelineAsync(eventType, period);
        }

        public async Task<List<(string Condition, int Count, double Percentage)>> GetCommonConditionsAsync(int accountId, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetCommonConditionsAsync(period);
        }

        public async Task<(List<(string Name, int Used, int Remaining)> MostUsedSupplies, List<(string Name, int Current, int Minimum)> LowStockAlerts, List<(string Name, string ExpiryDate, int Quantity)> ExpiringItems)> GetSupplyUsageAsync(int accountId, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetSupplyUsageAsync(period);
        }
        public async Task<(double OverallCoverage, List<(string CampaignName, int TargetCount, int CompletedCount, double CoverageRate, double ConsentRate)> ByCampaign, List<(string ClassName, double Coverage)> ByClass)> GetVaccinationCoverageAsync(int accountId, int? campaignId, int? classId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetVaccinationCoverageAsync(campaignId, classId);
        }

        public async Task<(List<(string Date, int Planned, int Actual)> Timeline, (int TotalPlanned, int TotalCompleted, double CompletionRate, double AveragePerDay, int DaysToComplete) Summary)> GetCampaignEffectivenessAsync(int accountId, int? campaignId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetCampaignEffectivenessAsync(campaignId);
        }
        public async Task<(int TotalBookings, int Completed, int Cancelled, double CompletionRate, List<(string Reason, int Count)> ByReason, List<(string TimeSlot, int Bookings)> ByTimeSlot)> GetConsultationStatisticsAsync(int accountId, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetConsultationStatisticsAsync(period);
        }

        public async Task<(int TotalRequests, int Approved, int Rejected, int Pending, double ApprovalRate, List<(string Medication, int Requests)> CommonMedications)> GetMedicationRequestsAsync(int accountId, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetMedicationRequestsAsync(period);
        }

        public async Task<(int TotalNurses, List<(string NurseName, int HealthChecks, int MedicalEvents, int Consultations, int WorkingDays, double AveragePerDay)> ByNurse, (int HealthChecks, int MedicalEvents, int Consultations, int MedicationApprovals) WorkloadDistribution)> GetNurseActivityAsync(int accountId, int? nurseId, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetNurseActivityAsync(nurseId, period);
        }
        public async Task<HealthSummaryData> GetHealthSummaryAsync(int accountId, string period)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetHealthSummaryAsync(period);
        }

        public async Task<ComparisonData> GetComparisonAsync(int accountId, string metric, string period1, string period2)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null || !account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is not active or not found.");
            }

            return await _dashboardRepo.GetComparisonAsync(metric, period1, period2);
        }
    }
}