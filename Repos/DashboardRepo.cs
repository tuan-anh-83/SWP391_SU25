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
    }
} 