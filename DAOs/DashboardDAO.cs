using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAOs
{
    public class DashboardDAO
    {
        private static DashboardDAO instance = null;
        private readonly DataContext _context;

        private DashboardDAO()
        {
            _context = new DataContext();
        }

        public static DashboardDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DashboardDAO();
                }
                return instance;
            }
        }

        public async Task<Dictionary<string, int>> GetDashboardOverviewAsync()
        {
            var currentMonth = DateTime.Now.AddMonths(-1);
            var lowStockThreshold = 20;
            var expiringThreshold = DateTime.Now.AddMonths(3);
            var overdueThreshold = DateTime.Now.AddMonths(-6);

            return new Dictionary<string, int>
            {
                { "TotalStudents", await _context.Students.AsNoTracking().CountAsync() },
                { "TotalNurses", await _context.Accounts.AsNoTracking().CountAsync(a => a.Role.RoleName == "Nurse") },
                { "TotalParents", await _context.Accounts.AsNoTracking().CountAsync(a => a.Role.RoleName == "Parent") },
                { "TotalClasses", await _context.Classes.AsNoTracking().CountAsync() },
                { "HealthChecksThisMonth", await _context.HealthChecks.AsNoTracking().CountAsync(h => h.Date >= currentMonth) },
                { "MedicalEventsThisMonth", await _context.MedicalEvents.AsNoTracking().CountAsync(m => m.Date >= currentMonth) },
                { "UpcomingVaccinations", await _context.VaccinationRecords.AsNoTracking().CountAsync(v => v.DateInjected >= DateTime.Now && v.Result == "Chưa tiêm") },
                { "ActiveConsultations", await _context.HealthConsultationBookings.AsNoTracking().CountAsync(c => c.Status == "Confirmed") },
                { "PendingMedicationRequests", await _context.ParentMedicationRequests.AsNoTracking().CountAsync(r => r.Status == "Pending") },
                { "LowStockItems", await _context.MedicalSupplies.AsNoTracking().CountAsync(s => s.Quantity < lowStockThreshold) },
                { "ExpiringMedications", await _context.MedicalSupplies.AsNoTracking().CountAsync(s => s.ExpiredDate != null && s.ExpiredDate <= expiringThreshold) },
                { "OverdueHealthChecks", await _context.HealthChecks.AsNoTracking().CountAsync(h => h.Date < overdueThreshold) }
            };
        }

        public async Task<Dictionary<string, List<TrendDataPoint>>> GetDashboardTrendsAsync(string period)
        {
            DateTime startDate;
            string groupByFormat;
            int takeLimit;

            switch (period.ToLower())
            {
                case "7days":
                    startDate = DateTime.Now.Date.AddDays(-6); // Bao gồm ngày hiện tại
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 7;
                    break;
                case "30days":
                    startDate = DateTime.Now.Date.AddDays(-29);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 30;
                    break;
                case "3months":
                    startDate = DateTime.Now.Date.AddMonths(-3);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 90; // Điều chỉnh để phù hợp với 3 tháng
                    break;
                case "1year":
                    startDate = DateTime.Now.Date.AddYears(-1);
                    groupByFormat = "yyyy-MM";
                    takeLimit = 12;
                    break;
                default:
                    throw new ArgumentException("Invalid period. Must be 7days, 30days, 3months, or 1year.");
            }

            var result = new Dictionary<string, List<TrendDataPoint>>();

            // Truy vấn HealthChecks
            var healthChecksQuery = _context.HealthChecks
                .AsNoTracking()
                .Where(h => h.Date != null && h.Date >= startDate);

            if (groupByFormat == "yyyy-MM")
            {
                var healthChecksData = await healthChecksQuery
                    .GroupBy(h => new { h.Date.Year, h.Date.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Year)
                    .ThenBy(t => t.Month)
                    .Take(takeLimit)
                    .ToListAsync();

                result["HealthChecks"] = healthChecksData
                    .Select(t => new TrendDataPoint
                    {
                        Date = new DateTime(t.Year, t.Month, 1).ToString("yyyy-MM"),
                        Count = t.Count
                    }).ToList();
            }
            else
            {
                var healthChecksData = await healthChecksQuery
                    .GroupBy(h => h.Date.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Date)
                    .Take(takeLimit)
                    .ToListAsync();

                result["HealthChecks"] = healthChecksData
                    .Select(t => new TrendDataPoint
                    {
                        Date = t.Date.ToString("yyyy-MM-dd"),
                        Count = t.Count
                    }).ToList();
            }

            // Truy vấn MedicalEvents
            var medicalEventsQuery = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date != null && m.Date >= startDate);

            if (groupByFormat == "yyyy-MM")
            {
                var medicalEventsData = await medicalEventsQuery
                    .GroupBy(m => new { m.Date.Year, m.Date.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Year)
                    .ThenBy(t => t.Month)
                    .Take(takeLimit)
                    .ToListAsync();

                result["MedicalEvents"] = medicalEventsData
                    .Select(t => new TrendDataPoint
                    {
                        Date = new DateTime(t.Year, t.Month, 1).ToString("yyyy-MM"),
                        Count = t.Count
                    }).ToList();
            }
            else
            {
                var medicalEventsData = await medicalEventsQuery
                    .GroupBy(m => m.Date.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Date)
                    .Take(takeLimit)
                    .ToListAsync();

                result["MedicalEvents"] = medicalEventsData
                    .Select(t => new TrendDataPoint
                    {
                        Date = t.Date.ToString("yyyy-MM-dd"),
                        Count = t.Count
                    }).ToList();
            }

            // Truy vấn Consultations
            var consultationsQuery = _context.HealthConsultationBookings
                .AsNoTracking()
                .Where(c => c.ScheduledTime != null && c.ScheduledTime >= startDate);

            if (groupByFormat == "yyyy-MM")
            {
                var consultationsData = await consultationsQuery
                    .GroupBy(c => new { c.ScheduledTime.Year, c.ScheduledTime.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Year)
                    .ThenBy(t => t.Month)
                    .Take(takeLimit)
                    .ToListAsync();

                result["Consultations"] = consultationsData
                    .Select(t => new TrendDataPoint
                    {
                        Date = new DateTime(t.Year, t.Month, 1).ToString("yyyy-MM"),
                        Count = t.Count
                    }).ToList();
            }
            else
            {
                var consultationsData = await consultationsQuery
                    .GroupBy(c => c.ScheduledTime.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Date)
                    .Take(takeLimit)
                    .ToListAsync();

                result["Consultations"] = consultationsData
                    .Select(t => new TrendDataPoint
                    {
                        Date = t.Date.ToString("yyyy-MM-dd"),
                        Count = t.Count
                    }).ToList();
            }

            // Truy vấn Vaccinations
            var vaccinationsQuery = _context.VaccinationRecords
                .AsNoTracking()
                .Where(v => v.DateInjected != null && v.DateInjected >= startDate);

            if (groupByFormat == "yyyy-MM")
            {
                var vaccinationsData = await vaccinationsQuery
                    .GroupBy(v => new { v.DateInjected.Year, v.DateInjected.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Year)
                    .ThenBy(t => t.Month)
                    .Take(takeLimit)
                    .ToListAsync();

                result["Vaccinations"] = vaccinationsData
                    .Select(t => new TrendDataPoint
                    {
                        Date = new DateTime(t.Year, t.Month, 1).ToString("yyyy-MM"),
                        Count = t.Count
                    }).ToList();
            }
            else
            {
                var vaccinationsData = await vaccinationsQuery
                    .GroupBy(v => v.DateInjected.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(t => t.Date)
                    .Take(takeLimit)
                    .ToListAsync();

                result["Vaccinations"] = vaccinationsData
                    .Select(t => new TrendDataPoint
                    {
                        Date = t.Date.ToString("yyyy-MM-dd"),
                        Count = t.Count
                    }).ToList();
            }

            return result;
        }


        public async Task<List<RecentActivityData>> GetRecentActivitiesAsync(int page, int pageSize, string? type)
        {
            var query = from s in _context.Students
                        join c in _context.Classes on s.ClassId equals c.ClassId
                        from source in
                            (from h in _context.HealthChecks
                             where type == null || type == "health_check"
                             select new { Type = "health_check", StudentId = h.StudentID, Timestamp = h.Date, Description = h.HealthCheckDescription })
                            .Concat(
                                from m in _context.MedicalEvents
                                where type == null || type == "medical_event"
                                select new { Type = "medical_event", StudentId = m.StudentId, Timestamp = m.Date, Description = m.Description })
                            .Concat(
                                from cb in _context.HealthConsultationBookings
                                where type == null || type == "consultation"
                                select new { Type = "consultation", StudentId = cb.StudentId, Timestamp = cb.ScheduledTime, Description = cb.Reason })
                        where s.StudentId == source.StudentId
                        select new RecentActivityData
                        {
                            Type = source.Type,
                            StudentName = s.Fullname,
                            ClassName = c.ClassName,
                            Timestamp = source.Timestamp,
                            Description = source.Description
                        };

            return await query
                .AsNoTracking()
                .OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<(Dictionary<string, int> byGender, Dictionary<string, int> byAge, List<(string className, int count)> byClass)> GetStudentDistributionAsync()
        {
            var byGender = await _context.Students
                .AsNoTracking()
                .GroupBy(s => s.Gender)
                .Select(g => new { Gender = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Gender, g => g.Count);

            var currentYear = DateTime.Now.Year;
            var studentsWithAge = await _context.Students
                .AsNoTracking()
                .Select(s => new { Age = currentYear - s.DateOfBirth.Year })
                .ToListAsync();

            var byAge = studentsWithAge
                .GroupBy(s => s.Age switch
                {
                    var age when age >= 6 && age <= 8 => "6-8",
                    var age when age >= 9 && age <= 11 => "9-11",
                    var age when age >= 12 && age <= 14 => "12-14",
                    var age when age >= 15 && age <= 18 => "15-18",
                    _ => "Other"
                })
                .Select(g => new { AgeGroup = g.Key, Count = g.Count() })
                .ToDictionary(g => g.AgeGroup, g => g.Count);

            var byClass = await _context.Classes
                .AsNoTracking()
                .Select(c => new
                {
                    className = c.ClassName,
                    count = c.Students.Count()
                })
                .OrderBy(c => c.className)
                .ToListAsync();

            return (byGender, byAge, byClass.Select(c => (c.className, c.count)).ToList());
        }
        public async Task<(List<(string month, double value, int count)> averageHeight, List<(string month, double value, int count)> averageWeight)> GetGrowthTrendsAsync(string period, string ageGroup)
        {
            DateTime startDate;
            string groupByFormat;
            int takeLimit;

            switch (period?.ToLower())
            {
                case "7days":
                    startDate = DateTime.Now.Date.AddDays(-6); // Bao gồm ngày hiện tại
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 7;
                    break;
                case "30days":
                    startDate = DateTime.Now.Date.AddDays(-29);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 30;
                    break;
                case "3months":
                    startDate = DateTime.Now.Date.AddMonths(-3);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 90;
                    break;
                case "1year":
                    startDate = DateTime.Now.Date.AddYears(-1);
                    groupByFormat = "yyyy-MM";
                    takeLimit = 12;
                    break;
                default:
                    startDate = DateTime.Now.Date.AddYears(-1);
                    groupByFormat = "yyyy-MM";
                    takeLimit = 12;
                    break;
            }

            int? minAge = null, maxAge = null;
            if (!string.IsNullOrEmpty(ageGroup) && ageGroup.Contains("-"))
            {
                var parts = ageGroup.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out int min) && int.TryParse(parts[1], out int max))
                {
                    minAge = min;
                    maxAge = max;
                }
            }

            var query = from hc in _context.HealthChecks
                        join s in _context.Students on hc.StudentID equals s.StudentId
                        where hc.Date >= startDate && hc.Date <= DateTime.Now.Date && hc.Height.HasValue && hc.Weight.HasValue
                        select new
                        {
                            hc.Date,
                            hc.Height,
                            hc.Weight,
                            hc.StudentID,
                            Age = DateTime.Now.Year - s.DateOfBirth.Year
                        };

            if (minAge.HasValue && maxAge.HasValue)
            {
                query = query.Where(x => x.Age >= minAge.Value && x.Age <= maxAge.Value);
            }

            List<(string month, double value, int count)> averageHeight, averageWeight;

            if (groupByFormat == "yyyy-MM")
            {
                var data = await query
                    .AsNoTracking()
                    .GroupBy(x => new { x.Date.Year, x.Date.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        AvgHeight = g.Average(x => x.Height.Value),
                        AvgWeight = g.Average(x => x.Weight.Value),
                        Count = g.Select(x => x.StudentID).Distinct().Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .Take(takeLimit)
                    .ToListAsync();

                var allMonths = Enumerable.Range(0, takeLimit)
                    .Select(i => startDate.AddMonths(i).Date)
                    .ToList();

                averageHeight = allMonths.Select(m =>
                {
                    var item = data.FirstOrDefault(d => d.Year == m.Year && d.Month == m.Month);
                    return (month: m.ToString("yyyy-MM"), value: item != null ? Math.Round(item.AvgHeight, 1) : 0.0, count: item != null ? item.Count : 0);
                }).ToList();

                averageWeight = allMonths.Select(m =>
                {
                    var item = data.FirstOrDefault(d => d.Year == m.Year && d.Month == m.Month);
                    return (month: m.ToString("yyyy-MM"), value: item != null ? Math.Round(item.AvgWeight, 1) : 0.0, count: item != null ? item.Count : 0);
                }).ToList();
            }
            else
            {
                var data = await query
                    .AsNoTracking()
                    .GroupBy(x => x.Date.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        AvgHeight = g.Average(x => x.Height.Value),
                        AvgWeight = g.Average(x => x.Weight.Value),
                        Count = g.Select(x => x.StudentID).Distinct().Count()
                    })
                    .OrderBy(x => x.Date)
                    .Take(takeLimit)
                    .ToListAsync();

                var allDays = Enumerable.Range(0, takeLimit)
                    .Select(i => startDate.AddDays(i).Date)
                    .ToList();

                averageHeight = allDays.Select(d =>
                {
                    var item = data.FirstOrDefault(x => x.Date.Date == d.Date);
                    return (month: d.ToString("yyyy-MM-dd"), value: item != null ? Math.Round(item.AvgHeight, 1) : 0.0, count: item != null ? item.Count : 0);
                }).ToList();

                averageWeight = allDays.Select(d =>
                {
                    var item = data.FirstOrDefault(x => x.Date.Date == d.Date);
                    return (month: d.ToString("yyyy-MM-dd"), value: item != null ? Math.Round(item.AvgWeight, 1) : 0.0, count: item != null ? item.Count : 0);
                }).ToList();
            }

            return (averageHeight, averageWeight);
        }
        public async Task<(List<(string Date, int Accidents, int Illnesses, int Fevers, int Others)> Timeline, Dictionary<string, int> TotalsByType)> GetMedicalEventsTimelineAsync(string eventType, string period)
        {
            DateTime startDate;
            string groupByFormat;
            int takeLimit;

            switch (period.ToLower())
            {
                case "30days":
                    startDate = DateTime.Now.Date.AddDays(-29);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 30;
                    break;
                case "3months":
                    startDate = DateTime.Now.Date.AddMonths(-3);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 90;
                    break;
                default:
                    throw new ArgumentException("Invalid period. Must be 30days or 3months.");
            }

            var query = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= startDate && m.Date <= DateTime.Now.Date);

            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(m => m.Type.ToLower() == eventType.ToLower());
            }

            var data = await query
                .GroupBy(m => m.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Accidents = g.Count(m => m.Type.ToLower() == "tai nạn"),
                    Illnesses = g.Count(m => m.Type.ToLower() == "dịch bệnh"),
                    Fevers = g.Count(m => m.Type.ToLower() == "sốt"),
                    Others = g.Count(m => m.Type.ToLower() == "khác")
                })
                .OrderBy(x => x.Date)
                .Take(takeLimit)
                .ToListAsync();

            var allDays = Enumerable.Range(0, takeLimit)
                .Select(i => startDate.AddDays(i).Date)
                .ToList();

            var timeline = allDays.Select(d =>
            {
                var item = data.FirstOrDefault(x => x.Date.Date == d.Date);
                return (
                    Date: d.ToString("yyyy-MM-dd"),
                    Accidents: item != null ? item.Accidents : 0,
                    Illnesses: item != null ? item.Illnesses : 0,
                    Fevers: item != null ? item.Fevers : 0,
                    Others: item != null ? item.Others : 0
                );
            }).ToList();

            var totals = new Dictionary<string, int>
    {
        { "accidents", data.Sum(x => x.Accidents) },
        { "illnesses", data.Sum(x => x.Illnesses) },
        { "fevers", data.Sum(x => x.Fevers) },
        { "others", data.Sum(x => x.Others) }
    };

            return (timeline, totals);
        }

        public async Task<List<(string Condition, int Count, double Percentage)>> GetCommonConditionsAsync(string period)
        {
            DateTime startDate = period.ToLower() == "3months" ? DateTime.Now.Date.AddMonths(-3) : throw new ArgumentException("Invalid period. Must be 3months.");

            var query = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= startDate && m.Date <= DateTime.Now.Date);

            var totalEvents = await query.CountAsync();
            totalEvents = totalEvents == 0 ? 1 : totalEvents; // Avoid division by zero

            var data = await query
                .GroupBy(m => m.Type) // Nhóm theo Type thay vì Description
                .Select(g => new
                {
                    Condition = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return data.Select(x => (
                Condition: x.Condition, // Sẽ là "tai nạn", "dịch bệnh", "sốt", hoặc "khác"
                Count: x.Count,
                Percentage: Math.Round((double)x.Count / totalEvents * 100, 2)
            )).ToList();
        }

        public async Task<(List<(string Name, int Used, int Remaining)> MostUsedSupplies, List<(string Name, int Current, int Minimum)> LowStockAlerts, List<(string Name, string ExpiryDate, int Quantity)> ExpiringItems)> GetSupplyUsageAsync(string period)
        {
            DateTime startDate = period.ToLower() == "1month" ? DateTime.Now.Date.AddMonths(-1) : throw new ArgumentException("Invalid period. Must be 1month.");
            DateTime expiryThreshold = DateTime.Now.AddMonths(3);
            int lowStockThreshold = 20;

            // Most Used Supplies
            var supplyUsageQuery = from me in _context.MedicalEvents
                                   join mems in _context.MedicalEventMedicalSupplies on me.MedicalEventId equals mems.MedicalEventId
                                   join ms in _context.MedicalSupplies on mems.MedicalSupplyId equals ms.MedicalSupplyId
                                   where me.Date >= startDate && me.Date <= DateTime.Now.Date
                                   group new { ms, mems } by ms.Name into g
                                   select new
                                   {
                                       Name = g.Key,
                                       Used = g.Sum(x => x.mems.QuantityUsed),
                                       Remaining = g.First().ms.Quantity
                                   };

            var mostUsedSupplies = await supplyUsageQuery
                .OrderByDescending(x => x.Used)
                .Take(10)
                .ToListAsync();

            // Low Stock Alerts
            var lowStockQuery = _context.MedicalSupplies
                .AsNoTracking()
                .Where(s => s.Quantity < lowStockThreshold)
                .Select(s => new
                {
                    Name = s.Name,
                    Current = s.Quantity,
                    Minimum = lowStockThreshold
                });

            var lowStockAlerts = await lowStockQuery
                .OrderBy(x => x.Current)
                .ToListAsync();

            // Expiring Items
            var expiringQuery = _context.MedicalSupplies
                .AsNoTracking()
                .Where(s => s.ExpiredDate != null && s.ExpiredDate <= expiryThreshold)
                .Select(s => new
                {
                    Name = s.Name,
                    ExpiryDate = s.ExpiredDate,
                    Quantity = s.Quantity
                });

            var expiringItems = await expiringQuery
                .OrderBy(x => x.ExpiryDate)
                .ToListAsync();

            return (
                MostUsedSupplies: mostUsedSupplies.Select(x => (x.Name, x.Used, x.Remaining)).ToList(),
                LowStockAlerts: lowStockAlerts.Select(x => (x.Name, x.Current, x.Minimum)).ToList(),
                ExpiringItems: expiringItems.Select(x => (x.Name, x.ExpiryDate?.ToString("yyyy-MM-dd") ?? "", x.Quantity)).ToList()
            );
        }
        public async Task<(double OverallCoverage, List<(string CampaignName, int TargetCount, int CompletedCount, double CoverageRate, double ConsentRate)> ByCampaign, List<(string ClassName, double Coverage)> ByClass)> GetVaccinationCoverageAsync(int? campaignId, int? classId)
        {
            var query = from vr in _context.VaccinationRecords
                        join vc in _context.VaccinationCampaigns on vr.CampaignId equals vc.CampaignId
                        join s in _context.Students on vr.StudentId equals s.StudentId
                        join c in _context.Classes on s.ClassId equals c.ClassId
                        select new { vr, vc, s, c };

            if (campaignId.HasValue)
            {
                query = query.Where(x => x.vr.CampaignId == campaignId.Value);
            }

            if (classId.HasValue)
            {
                query = query.Where(x => x.s.ClassId == classId.Value);
            }

            var records = await query.AsNoTracking().ToListAsync();

            // Overall Coverage
            var totalRecords = records.Count;
            var completedRecords = records.Count(r => r.vr.Result == "Đã tiêm");
            var overallCoverage = totalRecords > 0 ? Math.Round((double)completedRecords / totalRecords * 100, 2) : 0.0;

            // By Campaign
            var consentQuery = from cons in _context.VaccinationConsents
                               where cons.IsAgreed == true
                               select cons;

            if (campaignId.HasValue)
            {
                consentQuery = consentQuery.Where(cons => cons.CampaignId == campaignId.Value);
            }

            var consents = await consentQuery.AsNoTracking().ToListAsync();

            var byCampaign = await (from vc in _context.VaccinationCampaigns
                                    join vr in _context.VaccinationRecords on vc.CampaignId equals vr.CampaignId into recordsGroup
                                    from vr in recordsGroup.DefaultIfEmpty()
                                    join cons in _context.VaccinationConsents on vc.CampaignId equals cons.CampaignId into consentsGroup
                                    from cons in consentsGroup.DefaultIfEmpty()
                                    where !campaignId.HasValue || vc.CampaignId == campaignId.Value
                                    group new { vc, vr, cons } by new { vc.CampaignId, vc.Name } into g
                                    select new
                                    {
                                        CampaignId = g.Key.CampaignId,
                                        CampaignName = g.Key.Name,
                                        TargetCount = g.Count(x => x.cons != null),
                                        CompletedCount = g.Count(x => x.vr != null && x.vr.Result == "Đã tiêm"),
                                        ConsentCount = g.Count(x => x.cons != null && x.cons.IsAgreed == true)
                                    })
                                    .AsNoTracking()
                                    .ToListAsync();

            var byCampaignResult = byCampaign.Select(c => (
                CampaignName: c.CampaignName,
                TargetCount: c.TargetCount,
                CompletedCount: c.CompletedCount,
                CoverageRate: c.TargetCount > 0 ? Math.Round((double)c.CompletedCount / c.TargetCount * 100, 2) : 0.0,
                ConsentRate: c.TargetCount > 0 ? Math.Round((double)c.ConsentCount / c.TargetCount * 100, 2) : 0.0
            )).ToList();

            // By Class
            var byClass = await (from s in _context.Students
                                 join c in _context.Classes on s.ClassId equals c.ClassId
                                 join vr in _context.VaccinationRecords on s.StudentId equals vr.StudentId into recordsGroup
                                 from vr in recordsGroup.DefaultIfEmpty()
                                 where (!classId.HasValue || s.ClassId == classId.Value) &&
                                       (!campaignId.HasValue || vr.CampaignId == campaignId.Value)
                                 group new { s, c, vr } by new { c.ClassId, c.ClassName } into g
                                 select new
                                 {
                                     ClassId = g.Key.ClassId,
                                     ClassName = g.Key.ClassName,
                                     TotalStudents = g.Count(),
                                     CompletedCount = g.Count(x => x.vr != null && x.vr.Result == "Đã tiêm")
                                 })
                                 .AsNoTracking()
                                 .ToListAsync();

            var byClassResult = byClass.Select(c => (
                ClassName: c.ClassName,
                Coverage: c.TotalStudents > 0 ? Math.Round((double)c.CompletedCount / c.TotalStudents * 100, 2) : 0.0
            )).ToList();

            return (overallCoverage, byCampaignResult, byClassResult);
        }

        public async Task<(List<(string Date, int Planned, int Actual)> Timeline, (int TotalPlanned, int TotalCompleted, double CompletionRate, double AveragePerDay, int DaysToComplete) Summary)> GetCampaignEffectivenessAsync(int? campaignId)
        {
            var query = from vr in _context.VaccinationRecords
                        join vc in _context.VaccinationCampaigns on vr.CampaignId equals vc.CampaignId
                        where vr.DateInjected != null
                        select new { vr, vc };

            if (campaignId.HasValue)
            {
                query = query.Where(x => x.vr.CampaignId == campaignId.Value);
            }

            var records = await query.AsNoTracking().ToListAsync();

            // Timeline
            var minDate = records.Any() ? records.Min(r => r.vr.DateInjected.Date) : DateTime.Now.Date;
            var maxDate = records.Any() ? records.Max(r => r.vr.DateInjected.Date) : DateTime.Now.Date;
            var dateRange = Enumerable.Range(0, (maxDate - minDate).Days + 1)
                                      .Select(d => minDate.AddDays(d))
                                      .ToList();

            var plannedPerDay = await (from cons in _context.VaccinationConsents
                                       join vc in _context.VaccinationCampaigns on cons.CampaignId equals vc.CampaignId
                                       where cons.IsAgreed == true && cons.DateConfirmed != null &&
                                             (!campaignId.HasValue || cons.CampaignId == campaignId.Value)
                                       group cons by cons.DateConfirmed.Value.Date into g
                                       select new
                                       {
                                           Date = g.Key,
                                           Planned = g.Count()
                                       })
                                       .AsNoTracking()
                                       .ToListAsync();

            var actualPerDay = records
                .GroupBy(r => r.vr.DateInjected.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Actual = g.Count(r => r.vr.Result == "Đã tiêm")
                })
                .ToList();

            var timeline = dateRange.Select(d =>
            {
                var planned = plannedPerDay.FirstOrDefault(p => p.Date == d)?.Planned ?? 0;
                var actual = actualPerDay.FirstOrDefault(a => a.Date == d)?.Actual ?? 0;
                return (
                    Date: d.ToString("yyyy-MM-dd"),
                    Planned: planned,
                    Actual: actual
                );
            }).ToList();

            // Summary
            var totalPlanned = plannedPerDay.Sum(p => p.Planned);
            var totalCompleted = actualPerDay.Sum(a => a.Actual);
            var completionRate = totalPlanned > 0 ? Math.Round((double)totalCompleted / totalPlanned * 100, 2) : 0.0;
            var daysToComplete = dateRange.Count;
            var averagePerDay = daysToComplete > 0 ? Math.Round((double)totalCompleted / daysToComplete, 2) : 0.0;

            var summary = (
                TotalPlanned: totalPlanned,
                TotalCompleted: totalCompleted,
                CompletionRate: completionRate,
                AveragePerDay: averagePerDay,
                DaysToComplete: daysToComplete
            );

            return (timeline, summary);
        }
        public async Task<(int TotalBookings, int Completed, int Cancelled, double CompletionRate, List<(string Reason, int Count)> ByReason, List<(string TimeSlot, int Bookings)> ByTimeSlot)> GetConsultationStatisticsAsync(string period)
        {
            DateTime startDate = period.ToLower() == "1month" ? DateTime.Now.Date.AddMonths(-1) : throw new ArgumentException("Invalid period. Must be 1month.");

            var query = _context.HealthConsultationBookings
                .AsNoTracking()
                .Where(b => b.ScheduledTime >= startDate && b.ScheduledTime <= DateTime.Now.Date);

            var bookings = await query.ToListAsync();

            var totalBookings = bookings.Count;
            var completed = bookings.Count(b => b.Status == "Done");
            var cancelled = bookings.Count(b => b.Status == "Cancelled");
            var completionRate = totalBookings > 0 ? Math.Round((double)completed / totalBookings * 100, 2) : 0.0;

            var byReason = await query
                .Where(b => !string.IsNullOrEmpty(b.Reason))
                .GroupBy(b => b.Reason)
                .Select(g => new
                {
                    Reason = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToListAsync();

            var byTimeSlot = await query
                .GroupBy(b => b.ScheduledTime.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Bookings = g.Count()
                })
                .OrderBy(g => g.Hour)
                .ToListAsync();

            var timeSlots = byTimeSlot.Select(t => (
                TimeSlot: $"{t.Hour:00}:00-{(t.Hour + 1):00}:00",
                Bookings: t.Bookings
            )).ToList();

            return (
                TotalBookings: totalBookings,
                Completed: completed,
                Cancelled: cancelled,
                CompletionRate: completionRate,
                ByReason: byReason.Select(r => (r.Reason, r.Count)).ToList(),
                ByTimeSlot: timeSlots
            );
        }

        public async Task<(int TotalRequests, int Approved, int Rejected, int Pending, double ApprovalRate, List<(string Medication, int Requests)> CommonMedications)> GetMedicationRequestsAsync(string period)
        {
            DateTime startDate = period.ToLower() == "1month" ? DateTime.Now.Date.AddMonths(-1) : throw new ArgumentException("Invalid period. Must be 1month.");

            var query = _context.ParentMedicationRequests
                .AsNoTracking()
                .Where(r => r.DateCreated >= startDate && r.DateCreated <= DateTime.Now.Date);

            var requests = await query.ToListAsync();

            var totalRequests = requests.Count;
            var approved = requests.Count(r => r.Status == "Approved");
            var rejected = requests.Count(r => r.Status == "Rejected");
            var pending = requests.Count(r => r.Status == "Pending");
            var approvalRate = totalRequests > 0 ? Math.Round((double)approved / totalRequests * 100, 2) : 0.0;

            var commonMedications = await (from r in _context.ParentMedicationRequests
                                           join d in _context.ParentMedicationDetails on r.RequestId equals d.RequestId
                                           where r.DateCreated >= startDate && r.DateCreated <= DateTime.Now.Date
                                           group d by d.Name into g
                                           select new
                                           {
                                               Medication = g.Key,
                                               Requests = g.Count()
                                           })
                                          .OrderByDescending(g => g.Requests)
                                          .Take(10)
                                          .ToListAsync();

            return (
                TotalRequests: totalRequests,
                Approved: approved,
                Rejected: rejected,
                Pending: pending,
                ApprovalRate: approvalRate,
                CommonMedications: commonMedications.Select(m => (m.Medication, m.Requests)).ToList()
            );
        }

        public async Task<(int TotalNurses, List<(string NurseName, int HealthChecks, int MedicalEvents, int Consultations, int WorkingDays, double AveragePerDay)> ByNurse, (int HealthChecks, int MedicalEvents, int Consultations, int MedicationApprovals) WorkloadDistribution)> GetNurseActivityAsync(int? nurseId, string period)
        {
            DateTime startDate = period.ToLower() == "1month" ? DateTime.Now.Date.AddMonths(-1) : throw new ArgumentException("Invalid period. Must be 1month.");

            // Lấy danh sách y tá
            var nursesQuery = _context.Accounts
                .AsNoTracking()
                .Where(a => a.Role.RoleName == "Nurse");

            if (nurseId.HasValue)
            {
                nursesQuery = nursesQuery.Where(a => a.AccountID == nurseId.Value);
            }

            var nurses = await nursesQuery
                .Select(a => new { a.AccountID, a.Fullname })
                .ToListAsync();

            var totalNurses = nurses.Count;

            // Lấy dữ liệu HealthChecks - đảm bảo sử dụng NurseID
            var healthChecksQuery = _context.HealthChecks
                .AsNoTracking()
                .Where(h => h.Date >= startDate && h.Date <= DateTime.Now.Date)
                .Select(h => new { h.NurseID, h.Date });

            var healthChecksData = await healthChecksQuery
                .GroupBy(h => h.NurseID)
                .Select(g => new
                {
                    NurseId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Lấy danh sách ngày riêng cho HealthChecks
            var healthCheckDates = await healthChecksQuery
                .Select(h => new { h.NurseID, Date = h.Date.Date })
                .Distinct()
                .ToListAsync();

            // Lấy dữ liệu MedicalEvents - đảm bảo sử dụng NurseId
            var medicalEventsQuery = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= startDate && m.Date <= DateTime.Now.Date)
                .Select(m => new { m.NurseId, m.Date });

            var medicalEventsData = await medicalEventsQuery
                .GroupBy(m => m.NurseId)
                .Select(g => new
                {
                    NurseId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Lấy danh sách ngày riêng cho MedicalEvents
            var medicalEventDates = await medicalEventsQuery
                .Select(m => new { m.NurseId, Date = m.Date.Date })
                .Distinct()
                .ToListAsync();

            // Lấy dữ liệu Consultations - đảm bảo sử dụng NurseId
            var consultationsQuery = _context.HealthConsultationBookings
                .AsNoTracking()
                .Where(c => c.ScheduledTime >= startDate && c.ScheduledTime <= DateTime.Now.Date)
                .Select(c => new { c.NurseId, c.ScheduledTime });

            var consultationsData = await consultationsQuery
                .GroupBy(c => c.NurseId)
                .Select(g => new
                {
                    NurseId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Lấy danh sách ngày riêng cho Consultations
            var consultationDates = await consultationsQuery
                .Select(c => new { c.NurseId, Date = c.ScheduledTime.Date })
                .Distinct()
                .ToListAsync();

            // Lấy dữ liệu MedicationApprovals
            var medicationApprovalsQuery = _context.ParentMedicationRequests
                .AsNoTracking()
                .Where(r => r.DateCreated >= startDate && r.DateCreated <= DateTime.Now.Date && r.Status == "Approved");

            var medicationApprovalsCount = await medicationApprovalsQuery.CountAsync();

            // Xử lý dữ liệu ByNurse - đảm bảo mỗi y tá có dữ liệu riêng biệt
            var byNurseResult = new List<(string NurseName, int HealthChecks, int MedicalEvents, int Consultations, int WorkingDays, double AveragePerDay)>();

            foreach (var nurse in nurses)
            {
                try
                {
                    // Tìm dữ liệu cho từng y tá cụ thể
                    var healthChecks = healthChecksData.FirstOrDefault(h => h.NurseId == nurse.AccountID);
                    var medicalEvents = medicalEventsData.FirstOrDefault(m => m.NurseId == nurse.AccountID);
                    var consultations = consultationsData.FirstOrDefault(c => c.NurseId == nurse.AccountID);

                    var healthCheckCount = healthChecks?.Count ?? 0;
                    var medicalEventCount = medicalEvents?.Count ?? 0;
                    var consultationCount = consultations?.Count ?? 0;

                    // Tính số ngày làm việc cho từng y tá riêng biệt
                    var workingDays = healthCheckDates
                        .Where(h => h.NurseID == nurse.AccountID)
                        .Select(h => h.Date)
                        .Union(medicalEventDates.Where(m => m.NurseId == nurse.AccountID).Select(m => m.Date))
                        .Union(consultationDates.Where(c => c.NurseId == nurse.AccountID).Select(c => c.Date))
                        .Distinct()
                        .Count();

                    var totalActivities = healthCheckCount + medicalEventCount + consultationCount;
                    var averagePerDay = workingDays > 0 ? Math.Round((double)totalActivities / workingDays, 2) : 0.0;

                    var nurseData = (
                        NurseName: nurse.Fullname ?? "Unknown",
                        HealthChecks: healthCheckCount,
                        MedicalEvents: medicalEventCount,
                        Consultations: consultationCount,
                        WorkingDays: workingDays,
                        AveragePerDay: averagePerDay
                    );

                    byNurseResult.Add(nurseData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing nurse {nurse.AccountID} ({nurse.Fullname}): {ex.Message}");
                }
            }

            // Final validation
            // Console.WriteLine($"Final result count: {byNurseResult.Count}");
            // foreach (var result in byNurseResult)
            // {
            //     Console.WriteLine($"Final: {result.NurseName} - HC:{result.HealthChecks}, ME:{result.MedicalEvents}, C:{result.Consultations}");
            // }

            // Ensure we have the correct number of nurses
            // if (byNurseResult.Count != nurses.Count)
            // {
            //     Console.WriteLine($"Warning: Result count ({byNurseResult.Count}) doesn't match nurse count ({nurses.Count})");
            // }

            // Validate that each nurse has unique data
            var nurseNames = byNurseResult.Select(r => r.NurseName).Distinct().ToList();
            if (nurseNames.Count != byNurseResult.Count)
            {
                // Console.WriteLine($"Warning: Duplicate nurse names found. Unique names: {nurseNames.Count}, Total results: {byNurseResult.Count}");
            }

            // Tính WorkloadDistribution - tổng hợp tất cả hoạt động
            var workloadDistribution = (
                HealthChecks: healthChecksData.Sum(h => h.Count),
                MedicalEvents: medicalEventsData.Sum(m => m.Count),
                Consultations: consultationsData.Sum(c => c.Count),
                MedicationApprovals: medicationApprovalsCount
            );

            return (totalNurses, byNurseResult, workloadDistribution);
        }

        public async Task<HealthSummaryData> GetHealthSummaryAsync(string period)
        {
            DateTime startDate, previousStartDate;
            switch (period.ToLower())
            {
                case "monthly":
                    startDate = DateTime.Now.Date.AddMonths(-1);
                    previousStartDate = DateTime.Now.Date.AddMonths(-2);
                    break;
                case "quarterly":
                    startDate = DateTime.Now.Date.AddMonths(-3);
                    previousStartDate = DateTime.Now.Date.AddMonths(-6);
                    break;
                case "yearly":
                    startDate = DateTime.Now.Date.AddYears(-1);
                    previousStartDate = DateTime.Now.Date.AddYears(-2);
                    break;
                default:
                    throw new ArgumentException("Invalid period. Must be monthly, quarterly, or yearly.");
            }

            var endDate = DateTime.Now.Date;
            var previousEndDate = startDate.AddDays(-1);

            // Current period data
            var totalStudents = await _context.Students.AsNoTracking().CountAsync();

            var healthChecksQuery = _context.HealthChecks
                .AsNoTracking()
                .Where(h => h.Date >= startDate && h.Date <= endDate);

            var healthChecks = await healthChecksQuery.ToListAsync();
            var healthChecksCompleted = healthChecks.Count;
            var healthCheckCoverage = totalStudents > 0 ? Math.Round((double)healthChecksCompleted / totalStudents * 100, 2) : 0.0;

            var averageHeight = healthChecks.Any(h => h.Height.HasValue) ? Math.Round(healthChecks.Average(h => h.Height!.Value), 2) : 0.0;
            var averageWeight = healthChecks.Any(h => h.Weight.HasValue) ? Math.Round(healthChecks.Average(h => h.Weight!.Value), 2) : 0.0;
            var visionProblems = healthChecks.Count(h => h.LeftEye.HasValue && h.LeftEye.Value < 1.0 || h.RightEye.HasValue && h.RightEye.Value < 1.0);
            var visionProblemsRate = healthChecksCompleted > 0 ? Math.Round((double)visionProblems / healthChecksCompleted * 100, 2) : 0.0;

            var medicalEventsQuery = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= startDate && m.Date <= endDate);

            var medicalEvents = await medicalEventsQuery.ToListAsync();
            var medicalEventsTotal = medicalEvents.Count;
            var accidents = medicalEvents.Count(m => m.Type == "Tai nạn");
            var illnesses = medicalEvents.Count(m => m.Type == "Dịch bệnh");
            var fevers = medicalEvents.Count(m => m.Type == "Sốt");
            var others = medicalEvents.Count(m => m.Type == "Khác");

            // Previous period data for trends
            var prevHealthChecksQuery = _context.HealthChecks
                .AsNoTracking()
                .Where(h => h.Date >= previousStartDate && h.Date <= previousEndDate);

            var prevHealthChecks = await prevHealthChecksQuery.ToListAsync();
            var prevHealthChecksCompleted = prevHealthChecks.Count;
            var prevAverageHeight = prevHealthChecks.Any(h => h.Height.HasValue) ? Math.Round(prevHealthChecks.Average(h => h.Height!.Value), 2) : 0.0;
            var prevAverageWeight = prevHealthChecks.Any(h => h.Weight.HasValue) ? Math.Round(prevHealthChecks.Average(h => h.Weight!.Value), 2) : 0.0;

            var prevMedicalEventsQuery = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= previousStartDate && m.Date <= previousEndDate);

            var prevMedicalEvents = await prevMedicalEventsQuery.ToListAsync();
            var prevMedicalEventsTotal = prevMedicalEvents.Count;

            // Trends
            var healthChecksTrend = prevHealthChecksCompleted == 0 ? "Không đổi" :
                healthChecksCompleted > prevHealthChecksCompleted ? "Tăng" :
                healthChecksCompleted < prevHealthChecksCompleted ? "Giảm" : "Không đổi";

            var medicalEventsTrend = prevMedicalEventsTotal == 0 ? "Không đổi" :
                medicalEventsTotal > prevMedicalEventsTotal ? "Tăng" :
                medicalEventsTotal < prevMedicalEventsTotal ? "Giảm" : "Không đổi";

            var averageHeightTrend = prevAverageHeight == 0 ? "Không đổi" :
                averageHeight > prevAverageHeight ? "Tăng" :
                averageHeight < prevAverageHeight ? "Giảm" : "Không đổi";

            var averageWeightTrend = prevAverageWeight == 0 ? "Không đổi" :
                averageWeight > prevAverageWeight ? "Tăng" :
                averageWeight < prevAverageWeight ? "Giảm" : "Không đổi";

            return new HealthSummaryData
            {
                ReportPeriod = period,
                Summary = new SummaryData
                {
                    TotalStudents = totalStudents,
                    HealthChecksCompleted = healthChecksCompleted,
                    HealthCheckCoverage = healthCheckCoverage,
                    HealthIndicators = new HealthIndicatorsData
                    {
                        AverageHeight = averageHeight,
                        AverageWeight = averageWeight,
                        VisionProblemsRate = visionProblemsRate
                    },
                    MedicalEvents = new MedicalEventsData
                    {
                        Total = medicalEventsTotal,
                        Accidents = accidents,
                        Illnesses = illnesses,
                        Fevers = fevers,
                        Others = others
                    }
                },
                Trends = new TrendsData
                {
                    ComparedToPrevious = new ComparedToPreviousData
                    {
                        HealthChecks = healthChecksTrend,
                        MedicalEvents = medicalEventsTrend,
                        AverageHeight = averageHeightTrend,
                        AverageWeight = averageWeightTrend
                    }
                }
            };
        }

        public async Task<ComparisonData> GetComparisonAsync(string metric, string period1, string period2)
        {
            DateTime startDate1, endDate1, startDate2, endDate2;
            switch (period1.ToLower())
            {
                case "monthly":
                    startDate1 = DateTime.Now.Date.AddMonths(-1);
                    endDate1 = DateTime.Now.Date;
                    break;
                case "quarterly":
                    startDate1 = DateTime.Now.Date.AddMonths(-3);
                    endDate1 = DateTime.Now.Date;
                    break;
                case "yearly":
                    startDate1 = DateTime.Now.Date.AddYears(-1);
                    endDate1 = DateTime.Now.Date;
                    break;
                default:
                    throw new ArgumentException("Invalid period1. Must be monthly, quarterly, or yearly.");
            }

            switch (period2.ToLower())
            {
                case "monthly":
                    startDate2 = startDate1.AddMonths(-1);
                    endDate2 = startDate1.AddDays(-1);
                    break;
                case "quarterly":
                    startDate2 = startDate1.AddMonths(-3);
                    endDate2 = startDate1.AddDays(-1);
                    break;
                case "yearly":
                    startDate2 = startDate1.AddYears(-1);
                    endDate2 = startDate1.AddDays(-1);
                    break;
                default:
                    throw new ArgumentException("Invalid period2. Must be monthly, quarterly, or yearly.");
            }

            var period1Query = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= startDate1 && m.Date <= endDate1);

            var period1Events = await period1Query.ToListAsync();
            var period1Value = period1Events.Count;
            var period1Breakdown = new BreakdownData
            {
                Accidents = period1Events.Count(m => m.Type == "Tai nạn"),
                Illnesses = period1Events.Count(m => m.Type == "Dịch bệnh"),
                Fevers = period1Events.Count(m => m.Type == "Sốt"),
                Others = period1Events.Count(m => m.Type == "Khác")
            };

            var period2Query = _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= startDate2 && m.Date <= endDate2);

            var period2Events = await period2Query.ToListAsync();
            var period2Value = period2Events.Count;
            var period2Breakdown = new BreakdownData
            {
                Accidents = period2Events.Count(m => m.Type == "Tai nạn"),
                Illnesses = period2Events.Count(m => m.Type == "Dịch bệnh"),
                Fevers = period2Events.Count(m => m.Type == "Sốt"),
                Others = period2Events.Count(m => m.Type == "Khác")
            };

            var change = period1Value - period2Value;
            var changePercent = period2Value > 0 ? Math.Round((double)change / period2Value * 100, 2) : 0.0;
            var trend = change > 0 ? "Tăng" : change < 0 ? "Giảm" : "Không đổi";
            var significance = Math.Abs(changePercent) > 10 ? "Cao" : "Thấp";

            return new ComparisonData
            {
                Metric = metric,
                Period1 = new PeriodData
                {
                    Label = period1,
                    Value = period1Value,
                    Breakdown = period1Breakdown
                },
                Period2 = new PeriodData
                {
                    Label = period2,
                    Value = period2Value,
                    Breakdown = period2Breakdown
                },
                Comparison = new ComparisonResultData
                {
                    Change = change,
                    ChangePercent = changePercent,
                    Trend = trend,
                    Significance = significance
                }
            };
        }


        // Class hỗ trợ
        // Class hỗ trợ cho HealthSummary
        public class HealthSummaryData
        {
            public string ReportPeriod { get; set; }
            public SummaryData Summary { get; set; }
            public TrendsData Trends { get; set; }
        }

        public class SummaryData
        {
            public int TotalStudents { get; set; }
            public int HealthChecksCompleted { get; set; }
            public double HealthCheckCoverage { get; set; }
            public HealthIndicatorsData HealthIndicators { get; set; }
            public MedicalEventsData MedicalEvents { get; set; }
        }

        public class HealthIndicatorsData
        {
            public double AverageHeight { get; set; }
            public double AverageWeight { get; set; }
            public double VisionProblemsRate { get; set; }
        }

        public class MedicalEventsData
        {
            public int Total { get; set; }
            public int Accidents { get; set; }
            public int Illnesses { get; set; }
            public int Fevers { get; set; }
            public int Others { get; set; }
        }

        public class TrendsData
        {
            public ComparedToPreviousData ComparedToPrevious { get; set; }
        }

        public class ComparedToPreviousData
        {
            public string HealthChecks { get; set; }
            public string MedicalEvents { get; set; }
            public string AverageHeight { get; set; }
            public string AverageWeight { get; set; }
        }

        // Class hỗ trợ cho Comparison
        public class ComparisonData
        {
            public string Metric { get; set; }
            public PeriodData Period1 { get; set; }
            public PeriodData Period2 { get; set; }
            public ComparisonResultData Comparison { get; set; }
        }

        public class PeriodData
        {
            public string Label { get; set; }
            public int Value { get; set; }
            public BreakdownData Breakdown { get; set; }
        }

        public class BreakdownData
        {
            public int Accidents { get; set; }
            public int Illnesses { get; set; }
            public int Fevers { get; set; }
            public int Others { get; set; }
        }

        public class ComparisonResultData
        {
            public int Change { get; set; }
            public double ChangePercent { get; set; }
            public string Trend { get; set; }
            public string Significance { get; set; }
        }
        public class TrendDataPoint
        {
            public string Date { get; set; }
            public int Count { get; set; }
        }

        public class RecentActivityData
        {
            public string Type { get; set; }
            public string StudentName { get; set; }
            public string ClassName { get; set; }
            public DateTime? Timestamp { get; set; }
            public string Description { get; set; }
        }

    }
}