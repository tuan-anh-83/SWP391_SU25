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
    }
} 