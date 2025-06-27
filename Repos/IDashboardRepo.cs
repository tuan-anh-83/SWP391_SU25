using System.Security.Claims;
using static DAOs.DashboardDAO;

namespace Repos
{
    public interface IDashboardRepo
    {
        Task<Dictionary<string, int>> GetDashboardOverviewAsync();
        Task<Dictionary<string, List<TrendDataPoint>>> GetDashboardTrendsAsync(string period);
        Task<List<RecentActivityData>> GetRecentActivitiesAsync(int page, int pageSize, string? type);
    }
} 