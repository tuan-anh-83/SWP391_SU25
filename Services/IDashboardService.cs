using System.Security.Claims;
using static DAOs.DashboardDAO;


namespace Services
{
    public interface IDashboardService
    {
        Task<Dictionary<string, int>> GetOverviewAsync(int accountId);
        Task<Dictionary<string, List<TrendDataPoint>>> GetTrendsAsync(int accountId, string period);
        Task<List<RecentActivityData>> GetRecentActivitiesAsync(int accountId, int page, int pageSize, string? type);
    }
} 