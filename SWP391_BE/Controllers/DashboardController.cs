using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using SWP391_BE.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IAccountService _accountService;

        public DashboardController(IDashboardService dashboardService, IAccountService accountService)
        {
            _dashboardService = dashboardService;
            _accountService = accountService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                var overviewData = await _dashboardService.GetOverviewAsync(accountId);
                var overview = new DashboardOverviewDTO
                {
                    TotalStudents = overviewData["TotalStudents"],
                    TotalNurses = overviewData["TotalNurses"],
                    TotalParents = overviewData["TotalParents"],
                    TotalClasses = overviewData["TotalClasses"],
                    HealthChecksThisMonth = overviewData["HealthChecksThisMonth"],
                    MedicalEventsThisMonth = overviewData["MedicalEventsThisMonth"],
                    UpcomingVaccinations = overviewData["UpcomingVaccinations"],
                    ActiveConsultations = overviewData["ActiveConsultations"],
                    PendingMedicationRequests = overviewData["PendingMedicationRequests"],
                    LowStockItems = overviewData["LowStockItems"],
                    ExpiringMedications = overviewData["ExpiringMedications"],
                    OverdueHealthChecks = overviewData["OverdueHealthChecks"]
                };

                return Ok(overview);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("trends")]
        public async Task<IActionResult> GetTrends([FromQuery] string period)
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (string.IsNullOrEmpty(period) || !new[] { "7days", "30days", "3months", "1year" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be 7days, 30days, 3months, or 1year." });
                }

                var trendsData = await _dashboardService.GetTrendsAsync(accountId, period);
                var trends = new DashboardTrendsDTO
                {
                    HealthChecks = trendsData["HealthChecks"].Select(x => new TrendDataPointDTO
                    {
                        Date = x.Date,
                        Count = x.Count
                    }).ToList(),
                    MedicalEvents = trendsData["MedicalEvents"].Select(x => new TrendDataPointDTO
                    {
                        Date = x.Date,
                        Count = x.Count
                    }).ToList(),
                    Consultations = trendsData["Consultations"].Select(x => new TrendDataPointDTO
                    {
                        Date = x.Date,
                        Count = x.Count
                    }).ToList(),
                    Vaccinations = trendsData["Vaccinations"].Select(x => new TrendDataPointDTO
                    {
                        Date = x.Date,
                        Count = x.Count
                    }).ToList()
                };

                return Ok(trends);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        [HttpGet("recent-activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? type = null)
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (page < 1 || pageSize < 1)
                {
                    return BadRequest(new { message = "Page and pageSize must be positive numbers." });
                }

                if (type != null && !new[] { "health_check", "medical_event", "consultation", "vaccination" }.Contains(type.ToLower()))
                {
                    return BadRequest(new { message = "Invalid type. Must be health_check, medical_event, consultation, or vaccination." });
                }

                var activitiesData = await _dashboardService.GetRecentActivitiesAsync(accountId, page, pageSize, type);
                var activities = activitiesData.Select(x => new RecentActivityDTO
                {
                    Type = x.Type,
                    StudentName = x.StudentName,
                    ClassName = x.ClassName,
                    Timestamp = x.Timestamp ?? DateTime.MinValue, // X? lý null
                    Description = x.Description
                }).ToList();

                return Ok(activities);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}