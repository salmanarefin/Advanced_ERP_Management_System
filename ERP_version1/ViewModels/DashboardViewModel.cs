namespace ERP_version1.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }

        public int TotalDepartments { get; set; }

        public int TotalDesignations { get; set; }

        public int TodayAttendance { get; set; }

        public int PendingLeaves { get; set; }

        public int ApprovedLeaves { get; set; }

        public int RejectedLeaves { get; set; }

        public decimal CurrentMonthPayrollCost { get; set; }

        public List<string> DepartmentNames { get; set; } = new List<string>();

        public List<int> DepartmentEmployeeCounts { get; set; } = new List<int>();

        public List<string> AttendanceTrendLabels { get; set; } = new List<string>();

        public List<int> PresentTrendCounts { get; set; } = new List<int>();

        public List<int> AbsentTrendCounts { get; set; } = new List<int>();

        public List<int> LateTrendCounts { get; set; } = new List<int>();
    }
}