using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin,HR,Manager,PayrollOfficer")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> EmployeeReport(
            int? departmentId,
            string? status,
            string? searchTerm)
        {
            var employees = await _reportService.GetEmployeeReportAsync(
                departmentId,
                status,
                searchTerm);

            ViewBag.DepartmentId = departmentId;
            ViewBag.Status = status;
            ViewBag.SearchTerm = searchTerm;

            ViewBag.Departments = await _reportService.GetDepartmentSelectListAsync(departmentId);

            ViewBag.StatusList = new SelectList(new List<string>
            {
                "Active",
                "Inactive",
                "Resigned",
                "Terminated"
            }, status);

            ViewBag.TotalEmployees = employees.Count;
            ViewBag.ActiveEmployees = employees.Count(e => e.EmploymentStatus == "Active");
            ViewBag.InactiveEmployees = employees.Count(e => e.EmploymentStatus != "Active");

            return View(employees);
        }

        public async Task<IActionResult> AttendanceReport(
            DateTime? fromDate,
            DateTime? toDate,
            int? employeeId,
            string? status,
            string? searchTerm)
        {
            DateTime start = fromDate ?? DateTime.Today.AddDays(-30);
            DateTime end = toDate ?? DateTime.Today;

            var attendances = await _reportService.GetAttendanceReportAsync(
                fromDate,
                toDate,
                employeeId,
                status,
                searchTerm);

            ViewBag.FromDate = start.ToString("yyyy-MM-dd");
            ViewBag.ToDate = end.ToString("yyyy-MM-dd");
            ViewBag.EmployeeId = employeeId;
            ViewBag.Status = status;
            ViewBag.SearchTerm = searchTerm;

            ViewBag.Employees = await _reportService.GetEmployeeSelectListAsync(employeeId);

            ViewBag.StatusList = new SelectList(new List<string>
            {
                "Present",
                "Absent",
                "Late",
                "Half Day"
            }, status);

            ViewBag.TotalRecords = attendances.Count;
            ViewBag.PresentCount = attendances.Count(a => a.Status == "Present");
            ViewBag.AbsentCount = attendances.Count(a => a.Status == "Absent");
            ViewBag.LateCount = attendances.Count(a => a.Status == "Late" || a.LateMinutes > 0);
            ViewBag.TotalWorkingHours = attendances.Sum(a => a.WorkingHours);

            return View(attendances);
        }

        public async Task<IActionResult> LeaveReport(
            DateTime? fromDate,
            DateTime? toDate,
            int? employeeId,
            int? leaveTypeId,
            string? status,
            string? searchTerm)
        {
            DateTime start = fromDate ?? DateTime.Today.AddMonths(-1);
            DateTime end = toDate ?? DateTime.Today;

            var leaveApplications = await _reportService.GetLeaveReportAsync(
                fromDate,
                toDate,
                employeeId,
                leaveTypeId,
                status,
                searchTerm);

            ViewBag.FromDate = start.ToString("yyyy-MM-dd");
            ViewBag.ToDate = end.ToString("yyyy-MM-dd");
            ViewBag.EmployeeId = employeeId;
            ViewBag.LeaveTypeId = leaveTypeId;
            ViewBag.Status = status;
            ViewBag.SearchTerm = searchTerm;

            ViewBag.Employees = await _reportService.GetEmployeeSelectListAsync(employeeId);
            ViewBag.LeaveTypes = await _reportService.GetLeaveTypeSelectListAsync(leaveTypeId);

            ViewBag.StatusList = new SelectList(new List<string>
            {
                "Pending",
                "Approved",
                "Rejected"
            }, status);

            ViewBag.TotalApplications = leaveApplications.Count;
            ViewBag.PendingCount = leaveApplications.Count(l => l.Status == "Pending");
            ViewBag.ApprovedCount = leaveApplications.Count(l => l.Status == "Approved");
            ViewBag.RejectedCount = leaveApplications.Count(l => l.Status == "Rejected");
            ViewBag.TotalLeaveDays = leaveApplications.Sum(l => l.TotalDays);

            return View(leaveApplications);
        }

        public async Task<IActionResult> PayrollReport(
            int? month,
            int? year,
            int? employeeId,
            string? searchTerm)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            var payrolls = await _reportService.GetPayrollReportAsync(
                month,
                year,
                employeeId,
                searchTerm);

            ViewBag.Month = selectedMonth;
            ViewBag.Year = selectedYear;
            ViewBag.EmployeeId = employeeId;
            ViewBag.SearchTerm = searchTerm;

            ViewBag.Employees = await _reportService.GetEmployeeSelectListAsync(employeeId);

            ViewBag.TotalRecords = payrolls.Count;
            ViewBag.TotalBasicSalary = _reportService.CalculateTotalBasicSalary(payrolls);
            ViewBag.TotalBonus = _reportService.CalculateTotalBonus(payrolls);
            ViewBag.TotalDeduction = _reportService.CalculateTotalDeduction(payrolls);
            ViewBag.TotalTax = _reportService.CalculateTotalTax(payrolls);
            ViewBag.TotalPayroll = _reportService.CalculateTotalPayroll(payrolls);

            return View(payrolls);
        }
    }
}