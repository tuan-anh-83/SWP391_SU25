using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    startDate = DateTime.Now.AddDays(-7);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 7;
                    break;
                case "30days":
                    startDate = DateTime.Now.AddDays(-30);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 30;
                    break;
                case "3months":
                    startDate = DateTime.Now.AddMonths(-3);
                    groupByFormat = "yyyy-MM-dd";
                    takeLimit = 12;
                    break;
                case "1year":
                    startDate = DateTime.Now.AddYears(-1);
                    groupByFormat = "yyyy-MM";
                    takeLimit = 12;
                    break;
                default:
                    throw new ArgumentException("Invalid period. Must be 7days, 30days, 3months, or 1year.");
            }

            var result = new Dictionary<string, List<TrendDataPoint>>();

            var healthChecks = await _context.HealthChecks
                .AsNoTracking()
                .Where(h => h.Date >= startDate)
                .GroupBy(h => h.Date)
                .Select(g => new TrendDataPoint { Date = g.Key.ToString(groupByFormat), Count = g.Count() })
                .OrderBy(t => t.Date)
                .Take(takeLimit)
                .ToListAsync();

            result["HealthChecks"] = healthChecks;

            var medicalEvents = await _context.MedicalEvents
                .AsNoTracking()
                .Where(m => m.Date >= startDate)
                .GroupBy(m => m.Date)
                .Select(g => new TrendDataPoint { Date = g.Key.ToString(groupByFormat), Count = g.Count() })
                .OrderBy(t => t.Date)
                .Take(takeLimit)
                .ToListAsync();

            result["MedicalEvents"] = medicalEvents;

            var consultations = await _context.HealthConsultationBookings
                .AsNoTracking()
                .Where(c => c.ScheduledTime >= startDate)
                .GroupBy(c => c.ScheduledTime)
                .Select(g => new TrendDataPoint { Date = g.Key.ToString(groupByFormat), Count = g.Count() })
                .OrderBy(t => t.Date)
                .Take(takeLimit)
                .ToListAsync();

            result["Consultations"] = consultations;

            var vaccinations = await _context.VaccinationRecords
                .AsNoTracking()
                .Where(v => v.DateInjected >= startDate)
                .GroupBy(v => v.DateInjected)
                .Select(g => new TrendDataPoint { Date = g.Key.ToString(groupByFormat), Count = g.Count() })
                .OrderBy(t => t.Date)
                .Take(takeLimit)
                .ToListAsync();

            result["Vaccinations"] = vaccinations;

            return result;
        }

        public async Task<List<RecentActivityData>> GetRecentActivitiesAsync(int page, int pageSize, string? type)
        {
            var query = from s in _context.Students
                        join c in _context.Classes on s.ClassId equals c.ClassId
                        join h in _context.HealthChecks on s.StudentId equals h.StudentID into healthChecks
                        from h in healthChecks.DefaultIfEmpty()
                        join m in _context.MedicalEvents on s.StudentId equals m.StudentId into medicalEvents
                        from m in medicalEvents.DefaultIfEmpty()
                        join cb in _context.HealthConsultationBookings on s.StudentId equals cb.StudentId into consultations
                        from cb in consultations.DefaultIfEmpty()
                        join vr in _context.VaccinationRecords on s.StudentId equals vr.StudentId into vaccinations
                        from vr in vaccinations.DefaultIfEmpty()
                        where (type == null ||
                               (type == "health_check" && h != null) ||
                               (type == "medical_event" && m != null) ||
                               (type == "consultation" && cb != null) ||
                               (type == "vaccination" && vr != null))
                        select new RecentActivityData
                        {
                            Type = h != null ? "health_check" :
                                   m != null ? "medical_event" :
                                   cb != null ? "consultation" :
                                   vr != null ? "vaccination" : null,
                            StudentName = s.Fullname,
                            ClassName = c.ClassName,
                            Timestamp = (DateTime?)(h != null ? h.Date :
                                                  m != null ? m.Date :
                                                  cb != null ? cb.ScheduledTime :
                                                  vr != null ? vr.DateInjected : null),
                            Description = h != null ? h.HealthCheckDescription :
                                         m != null ? m.Description :
                                         cb != null ? cb.Reason :
                                         vr != null ? $"Tiêm chủng {vr.Result}" : null
                        };

            return await query
                .AsNoTracking()
                .Where(x => x.Type != null)
                .OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Class hỗ trợ
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