namespace SWP391_BE.DTO
{
    public class DashboardOverviewDTO
    {
        public int TotalStudents { get; set; }
        public int TotalNurses { get; set; }
        public int TotalParents { get; set; }
        public int TotalClasses { get; set; }
        public int HealthChecksThisMonth { get; set; }
        public int MedicalEventsThisMonth { get; set; }
        public int UpcomingVaccinations { get; set; }
        public int ActiveConsultations { get; set; }
        public int PendingMedicationRequests { get; set; }
        public int LowStockItems { get; set; }
        public int ExpiringMedications { get; set; }
        public int OverdueHealthChecks { get; set; }
    }

    public class TrendDataPointDTO
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public class DashboardTrendsDTO
    {
        public List<TrendDataPointDTO> HealthChecks { get; set; } = new List<TrendDataPointDTO>();
        public List<TrendDataPointDTO> MedicalEvents { get; set; } = new List<TrendDataPointDTO>();
        public List<TrendDataPointDTO> Consultations { get; set; } = new List<TrendDataPointDTO>();
        public List<TrendDataPointDTO> Vaccinations { get; set; } = new List<TrendDataPointDTO>();
    }

    public class RecentActivityDTO
    {
        public string Type { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
    }
}