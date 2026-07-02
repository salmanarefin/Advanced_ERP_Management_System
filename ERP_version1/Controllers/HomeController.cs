using ERP_version1.Data;
using ERP_version1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            DateTime today = DateTime.Today;
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year;

            DateTime attendanceStartDate = today.AddDays(-6);

            var departmentChartData = await _context.Departments
                .GroupJoin(
                    _context.Employees,
                    department => department.DepartmentId,
                    employee => employee.DepartmentId,
                    (department, employees) => new
                    {
                        DepartmentName = department.DepartmentName,
                        EmployeeCount = employees.Count()
                    })
                .OrderBy(x => x.DepartmentName)
                .ToListAsync();

            var attendanceTrendRawData = await _context.Attendances
                .Where(a => a.AttendanceDate.Date >= attendanceStartDate.Date &&
                            a.AttendanceDate.Date <= today.Date)
                .Select(a => new
                {
                    a.AttendanceDate,
                    a.Status,
                    a.LateMinutes
                })
                .ToListAsync();

            List<string> attendanceTrendLabels = new List<string>();
            List<int> presentTrendCounts = new List<int>();
            List<int> absentTrendCounts = new List<int>();
            List<int> lateTrendCounts = new List<int>();

            for (int i = 0; i < 7; i++)
            {
                DateTime date = attendanceStartDate.AddDays(i);

                var dayRecords = attendanceTrendRawData
                    .Where(a => a.AttendanceDate.Date == date.Date)
                    .ToList();

                attendanceTrendLabels.Add(date.ToString("dd MMM"));

                presentTrendCounts.Add(dayRecords.Count(a => a.Status == "Present"));

                absentTrendCounts.Add(dayRecords.Count(a => a.Status == "Absent"));

                lateTrendCounts.Add(dayRecords.Count(a =>
                    a.Status == "Late" || a.LateMinutes > 0));
            }

            DashboardViewModel model = new DashboardViewModel
            {
                TotalEmployees = await _context.Employees.CountAsync(),

                TotalDepartments = await _context.Departments.CountAsync(),

                TotalDesignations = await _context.Designations.CountAsync(),

                TodayAttendance = await _context.Attendances
                    .CountAsync(a => a.AttendanceDate.Date == today),

                PendingLeaves = await _context.LeaveApplications
                    .CountAsync(l => l.Status == "Pending"),

                ApprovedLeaves = await _context.LeaveApplications
                    .CountAsync(l => l.Status == "Approved"),

                RejectedLeaves = await _context.LeaveApplications
                    .CountAsync(l => l.Status == "Rejected"),

                CurrentMonthPayrollCost = await _context.Payrolls
                    .Where(p => p.Month == currentMonth && p.Year == currentYear)
                    .SumAsync(p => p.NetSalary),

                DepartmentNames = departmentChartData
                    .Select(x => x.DepartmentName)
                    .ToList(),

                DepartmentEmployeeCounts = departmentChartData
                    .Select(x => x.EmployeeCount)
                    .ToList(),

                AttendanceTrendLabels = attendanceTrendLabels,

                PresentTrendCounts = presentTrendCounts,

                AbsentTrendCounts = absentTrendCounts,

                LateTrendCounts = lateTrendCounts
            };

            return View(model);
        }
    }
}