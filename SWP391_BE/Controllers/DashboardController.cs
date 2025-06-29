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
                    Timestamp = x.Timestamp ?? DateTime.MinValue, // X? l? null
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

        [HttpGet("analytics/students/distribution")]
        public async Task<IActionResult> GetStudentDistribution()
        {
            try
            {
                var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                var (byGender, byAge, byClass) = await _dashboardService.GetStudentDistributionAsync(accountId);
                var distribution = new StudentDistributionDTO
                {
                    ByGender = byGender,
                    ByAge = byAge,
                    ByClass = byClass.Select(c => new ClassDistributionDTO
                    {
                        ClassName = c.className,
                        Count = c.count
                    }).ToList()
                };

                return Ok(distribution);
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

        [HttpGet("analytics/students/growth-trends")]
        public async Task<IActionResult> GetGrowthTrends([FromQuery] string period = "1year", [FromQuery] string? ageGroup = null)
        {
            try
            {
                var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                var (averageHeight, averageWeight) = await _dashboardService.GetGrowthTrendsAsync(accountId, period, ageGroup);
                var trends = new GrowthTrendsDTO
                {
                    AverageHeight = averageHeight.Select(x => new GrowthTrendDataPointDTO
                    {
                        Month = x.month,
                        Value = x.value,
                        Count = x.count
                    }).ToList(),
                    AverageWeight = averageWeight.Select(x => new GrowthTrendDataPointDTO
                    {
                        Month = x.month,
                        Value = x.value,
                        Count = x.count
                    }).ToList()
                };

                return Ok(trends);
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
        // API: Th?ng kê s? ki?n y t? theo th?i gian
        [HttpGet("analytics/medical/events-timeline")]
        public async Task<IActionResult> GetMedicalEventsTimeline([FromQuery] string eventType = null, [FromQuery] string period = "30days")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "30days", "3months" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be 30days or 3months." });
                }

                if (eventType != null && !new[] { "tai n?n", "d?ch b?nh", "s?t", "khác" }.Contains(eventType.ToLower()))
                {
                    return BadRequest(new { message = "Invalid eventType. Must be tai n?n, d?ch b?nh, s?t, or khác." });
                }

                var timelineData = await _dashboardService.GetMedicalEventsTimelineAsync(accountId, eventType, period);
                var response = new
                {
                    Timeline = timelineData.Timeline.Select(t => new
                    {
                        Date = t.Date,
                        Accidents = t.Accidents,
                        Illnesses = t.Illnesses,
                        Fevers = t.Fevers,
                        Others = t.Others
                    }).ToList(),
                    TotalsByType = new
                    {
                        Accidents = timelineData.TotalsByType["accidents"],
                        Illnesses = timelineData.TotalsByType["illnesses"],
                        Fevers = timelineData.TotalsByType["fevers"],
                        Others = timelineData.TotalsByType["others"]
                    }
                };

                return Ok(response);
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

        // API: Top b?nh t?t th??ng g?p
        [HttpGet("analytics/medical/common-conditions")]
        public async Task<IActionResult> GetCommonConditions([FromQuery] string period = "3months")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "3months" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be 3months." });
                }

                var conditionsData = await _dashboardService.GetCommonConditionsAsync(accountId, period);
                var response = conditionsData.Select(c => new
                {
                    Condition = c.Condition,
                    Count = c.Count,
                    Percentage = c.Percentage
                }).ToList();

                return Ok(response);
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

        // API: Th?ng kê s? d?ng v?t t? y t?
        [HttpGet("analytics/medical/supply-usage")]
        public async Task<IActionResult> GetSupplyUsage([FromQuery] string period = "1month")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "1month" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be 1month." });
                }

                var supplyData = await _dashboardService.GetSupplyUsageAsync(accountId, period);
                var response = new
                {
                    MostUsedSupplies = supplyData.MostUsedSupplies.Select(s => new
                    {
                        Name = s.Name,
                        Used = s.Used,
                        Remaining = s.Remaining
                    }).ToList(),
                    LowStockAlerts = supplyData.LowStockAlerts.Select(s => new
                    {
                        Name = s.Name,
                        Current = s.Current,
                        Minimum = s.Minimum
                    }).ToList(),
                    ExpiringItems = supplyData.ExpiringItems.Select(s => new
                    {
                        Name = s.Name,
                        ExpiryDate = s.ExpiryDate,
                        Quantity = s.Quantity
                    }).ToList()
                };

                return Ok(response);
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
        // API: T? l? tiêm ch?ng theo chi?n d?ch và l?p
        [HttpGet("analytics/vaccination/coverage")]
        public async Task<IActionResult> GetVaccinationCoverage([FromQuery] int? campaignId = null, [FromQuery] int? classId = null)
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                var coverageData = await _dashboardService.GetVaccinationCoverageAsync(accountId, campaignId, classId);
                var response = new
                {
                    OverallCoverage = coverageData.OverallCoverage,
                    ByCampaign = coverageData.ByCampaign.Select(c => new
                    {
                        CampaignName = c.CampaignName,
                        TargetCount = c.TargetCount,
                        CompletedCount = c.CompletedCount,
                        CoverageRate = c.CoverageRate,
                        ConsentRate = c.ConsentRate
                    }).ToList(),
                    ByClass = coverageData.ByClass.Select(c => new
                    {
                        ClassName = c.ClassName,
                        Coverage = c.Coverage
                    }).ToList()
                };

                return Ok(response);
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

        // API: Hi?u qu? chi?n d?ch tiêm ch?ng
        [HttpGet("analytics/vaccination/campaign-effectiveness")]
        public async Task<IActionResult> GetCampaignEffectiveness([FromQuery] int? campaignId = null)
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                var effectivenessData = await _dashboardService.GetCampaignEffectivenessAsync(accountId, campaignId);
                var response = new
                {
                    Timeline = effectivenessData.Timeline.Select(t => new
                    {
                        Date = t.Date,
                        Planned = t.Planned,
                        Actual = t.Actual
                    }).ToList(),
                    Summary = new
                    {
                        TotalPlanned = effectivenessData.Summary.TotalPlanned,
                        TotalCompleted = effectivenessData.Summary.TotalCompleted,
                        CompletionRate = effectivenessData.Summary.CompletionRate,
                        AveragePerDay = effectivenessData.Summary.AveragePerDay,
                        DaysToComplete = effectivenessData.Summary.DaysToComplete
                    }
                };

                return Ok(response);
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

        // API: Th?ng kê l?ch h?n t? v?n
        [HttpGet("analytics/consultations/statistics-in-1month")]
        public async Task<IActionResult> GetConsultationStatistics([FromQuery] string period = "1month")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "1month" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be 1month." });
                }

                var statsData = await _dashboardService.GetConsultationStatisticsAsync(accountId, period);
                var response = new
                {
                    TotalBookings = statsData.TotalBookings,
                    Completed = statsData.Completed,
                    Cancelled = statsData.Cancelled,
                    CompletionRate = statsData.CompletionRate,
                    ByReason = statsData.ByReason.Select(r => new
                    {
                        Reason = r.Reason,
                        Count = r.Count
                    }).ToList(),
                    ByTimeSlot = statsData.ByTimeSlot.Select(t => new
                    {
                        TimeSlot = t.TimeSlot,
                        Bookings = t.Bookings
                    }).ToList()
                };

                return Ok(response);
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

        // API: Th?ng kê yêu c?u gui thu?c
        [HttpGet("analytics/medication/requests-in-1month")]
        public async Task<IActionResult> GetMedicationRequests([FromQuery] string period = "1month")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "1month" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be 1month." });
                }

                var requestsData = await _dashboardService.GetMedicationRequestsAsync(accountId, period);
                var response = new
                {
                    TotalRequests = requestsData.TotalRequests,
                    Approved = requestsData.Approved,
                    Rejected = requestsData.Rejected,
                    Pending = requestsData.Pending,
                    ApprovalRate = requestsData.ApprovalRate,
                    CommonMedications = requestsData.CommonMedications.Select(m => new
                    {
                        Medication = m.Medication,
                        Requests = m.Requests
                    }).ToList()
                };

                return Ok(response);
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

        // API: Th?ng kê ho?t ??ng c?a y tá
        [HttpGet("analytics/nurses/activity-in-1month")]
        public async Task<IActionResult> GetNurseActivity([FromQuery] int? nurseId = null, [FromQuery] string period = "1month")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "1month" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be 1month." });
                }

                var activityData = await _dashboardService.GetNurseActivityAsync(accountId, nurseId, period);
                var response = new
                {
                    TotalNurses = activityData.TotalNurses,
                    ByNurse = activityData.ByNurse.Select(n => new
                    {
                        NurseName = n.NurseName,
                        HealthChecks = n.HealthChecks,
                        MedicalEvents = n.MedicalEvents,
                        Consultations = n.Consultations,
                        WorkingDays = n.WorkingDays,
                        AveragePerDay = n.AveragePerDay
                    }).ToList(),
                    WorkloadDistribution = new
                    {
                        HealthChecks = activityData.WorkloadDistribution.HealthChecks,
                        MedicalEvents = activityData.WorkloadDistribution.MedicalEvents,
                        Consultations = activityData.WorkloadDistribution.Consultations,
                        MedicationApprovals = activityData.WorkloadDistribution.MedicationApprovals
                    }
                };

                return Ok(response);
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

        // API: Báo cáo s?c kh?e ??nh k?
        [HttpGet("reports/health-summary")]
        public async Task<IActionResult> GetHealthSummary([FromQuery] string period = "monthly", [FromQuery] string format = "summary")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "monthly", "quarterly", "yearly" }.Contains(period.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be monthly, quarterly, or yearly." });
                }

                if (!new[] { "summary" }.Contains(format.ToLower()))
                {
                    return BadRequest(new { message = "Invalid format. Must be summary." });
                }

                var summaryData = await _dashboardService.GetHealthSummaryAsync(accountId, period);
                var response = new
                {
                    ReportPeriod = summaryData.ReportPeriod,
                    Summary = new
                    {
                        TotalStudents = summaryData.Summary.TotalStudents,
                        HealthChecksCompleted = summaryData.Summary.HealthChecksCompleted,
                        HealthCheckCoverage = summaryData.Summary.HealthCheckCoverage,
                        HealthIndicators = new
                        {
                            AverageHeight = summaryData.Summary.HealthIndicators.AverageHeight,
                            AverageWeight = summaryData.Summary.HealthIndicators.AverageWeight,
                            VisionProblemsRate = summaryData.Summary.HealthIndicators.VisionProblemsRate
                        },
                        MedicalEvents = new
                        {
                            Total = summaryData.Summary.MedicalEvents.Total,
                            Accidents = summaryData.Summary.MedicalEvents.Accidents,
                            Illnesses = summaryData.Summary.MedicalEvents.Illnesses,
                            Fevers = summaryData.Summary.MedicalEvents.Fevers,
                            Others = summaryData.Summary.MedicalEvents.Others
                        }
                    },
                    Trends = new
                    {
                        ComparedToPrevious = new
                        {
                            HealthChecks = summaryData.Trends.ComparedToPrevious.HealthChecks,
                            MedicalEvents = summaryData.Trends.ComparedToPrevious.MedicalEvents,
                            AverageHeight = summaryData.Trends.ComparedToPrevious.AverageHeight,
                            AverageWeight = summaryData.Trends.ComparedToPrevious.AverageWeight
                        }
                    }
                };

                return Ok(response);
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

        // API: So sánh ch? s? gi?a hai kho?ng th?i gian
        [HttpGet("analytics/comparison")]
        public async Task<IActionResult> GetComparison([FromQuery] string metric = "health_events", [FromQuery] string period1 = "monthly", [FromQuery] string period2 = "monthly")
        {
            try
            {
                var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                if (!new[] { "health_events" }.Contains(metric.ToLower()))
                {
                    return BadRequest(new { message = "Invalid metric. Must be health_events." });
                }

                if (!new[] { "monthly", "quarterly", "yearly" }.Contains(period1.ToLower()) ||
                    !new[] { "monthly", "quarterly", "yearly" }.Contains(period2.ToLower()))
                {
                    return BadRequest(new { message = "Invalid period. Must be monthly, quarterly, or yearly." });
                }

                var comparisonData = await _dashboardService.GetComparisonAsync(accountId, metric, period1, period2);
                var response = new
                {
                    Metric = comparisonData.Metric,
                    Period1 = new
                    {
                        Label = comparisonData.Period1.Label,
                        Value = comparisonData.Period1.Value,
                        Breakdown = new
                        {
                            Accidents = comparisonData.Period1.Breakdown.Accidents,
                            Illnesses = comparisonData.Period1.Breakdown.Illnesses,
                            Fevers = comparisonData.Period1.Breakdown.Fevers,
                            Others = comparisonData.Period1.Breakdown.Others
                        }
                    },
                    Period2 = new
                    {
                        Label = comparisonData.Period2.Label,
                        Value = comparisonData.Period2.Value,
                        Breakdown = new
                        {
                            Accidents = comparisonData.Period2.Breakdown.Accidents,
                            Illnesses = comparisonData.Period2.Breakdown.Illnesses,
                            Fevers = comparisonData.Period2.Breakdown.Fevers,
                            Others = comparisonData.Period2.Breakdown.Others
                        }
                    },
                    Comparison = new
                    {
                        Change = comparisonData.Comparison.Change,
                        ChangePercent = comparisonData.Comparison.ChangePercent,
                        Trend = comparisonData.Comparison.Trend,
                        Significance = comparisonData.Comparison.Significance
                    }
                };

                return Ok(response);
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