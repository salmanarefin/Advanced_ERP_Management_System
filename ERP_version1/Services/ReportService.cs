using ERP_version1.Data;
using ERP_version1.Models.AttendanceModule;
using ERP_version1.Models.HR;
using ERP_version1.Models.LeaveModule;
using ERP_version1.Models.PayrollModule;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetEmployeeReportAsync(
            int? departmentId,
            string? status,
            string? searchTerm)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .AsQueryable();

            if (departmentId != null && departmentId > 0)
            {
                query = query.Where(e => e.DepartmentId == departmentId);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e => e.EmploymentStatus == status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.EmployeeCode.Contains(searchTerm) ||
                    e.FullName.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm) ||
                    (e.Phone != null && e.Phone.Contains(searchTerm)) ||
                    (e.Department != null && e.Department.DepartmentName.Contains(searchTerm)) ||
                    (e.Designation != null && e.Designation.DesignationName.Contains(searchTerm))
                );
            }

            return await query
                .OrderBy(e => e.Department!.DepartmentName)
                .ThenBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<SelectList> GetDepartmentSelectListAsync(object? selectedValue = null)
        {
            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            return new SelectList(departments, "DepartmentId", "DepartmentName", selectedValue);
        }

        public async Task<SelectList> GetEmployeeSelectListAsync(object? selectedValue = null)
        {
            var employees = await _context.Employees
                .Where(e => e.EmploymentStatus == "Active")
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return new SelectList(employees, "EmployeeId", "FullName", selectedValue);
        }

        public async Task<SelectList> GetLeaveTypeSelectListAsync(object? selectedValue = null)
        {
            var leaveTypes = await _context.LeaveTypes
                .Where(l => l.IsActive)
                .OrderBy(l => l.LeaveTypeName)
                .ToListAsync();

            return new SelectList(leaveTypes, "LeaveTypeId", "LeaveTypeName", selectedValue);
        }

        public async Task<List<Attendance>> GetAttendanceReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            int? employeeId,
            string? status,
            string? searchTerm)
        {
            DateTime start = fromDate ?? DateTime.Today.AddDays(-30);
            DateTime end = toDate ?? DateTime.Today;

            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            query = query.Where(a =>
                a.AttendanceDate.Date >= start.Date &&
                a.AttendanceDate.Date <= end.Date);

            if (employeeId != null && employeeId > 0)
            {
                query = query.Where(a => a.EmployeeId == employeeId);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a =>
                    a.Employee != null &&
                    (
                        a.Employee.EmployeeCode.Contains(searchTerm) ||
                        a.Employee.FullName.Contains(searchTerm) ||
                        a.Employee.Email.Contains(searchTerm)
                    )
                );
            }

            return await query
                .OrderByDescending(a => a.AttendanceDate)
                .ThenBy(a => a.Employee!.FullName)
                .ToListAsync();
        }

        public async Task<List<LeaveApplication>> GetLeaveReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            int? employeeId,
            int? leaveTypeId,
            string? status,
            string? searchTerm)
        {
            DateTime start = fromDate ?? DateTime.Today.AddMonths(-1);
            DateTime end = toDate ?? DateTime.Today;

            var query = _context.LeaveApplications
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .AsQueryable();

            query = query.Where(l =>
                l.StartDate.Date >= start.Date &&
                l.StartDate.Date <= end.Date);

            if (employeeId != null && employeeId > 0)
            {
                query = query.Where(l => l.EmployeeId == employeeId);
            }

            if (leaveTypeId != null && leaveTypeId > 0)
            {
                query = query.Where(l => l.LeaveTypeId == leaveTypeId);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(l => l.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(l =>
                    (l.Reason != null && l.Reason.Contains(searchTerm)) ||
                    (l.RejectionReason != null && l.RejectionReason.Contains(searchTerm)) ||
                    (
                        l.Employee != null &&
                        (
                            l.Employee.EmployeeCode.Contains(searchTerm) ||
                            l.Employee.FullName.Contains(searchTerm) ||
                            l.Employee.Email.Contains(searchTerm)
                        )
                    ) ||
                    (
                        l.LeaveType != null &&
                        l.LeaveType.LeaveTypeName.Contains(searchTerm)
                    )
                );
            }

            return await query
                .OrderByDescending(l => l.AppliedAt)
                .ThenBy(l => l.Employee!.FullName)
                .ToListAsync();
        }

        public async Task<List<Payroll>> GetPayrollReportAsync(
            int? month,
            int? year,
            int? employeeId,
            string? searchTerm)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            var query = _context.Payrolls
                .Include(p => p.Employee)
                .AsQueryable();

            query = query.Where(p =>
                p.Month == selectedMonth &&
                p.Year == selectedYear);

            if (employeeId != null && employeeId > 0)
            {
                query = query.Where(p => p.EmployeeId == employeeId);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Employee != null &&
                    (
                        p.Employee.EmployeeCode.Contains(searchTerm) ||
                        p.Employee.FullName.Contains(searchTerm) ||
                        p.Employee.Email.Contains(searchTerm)
                    )
                );
            }

            return await query
                .OrderBy(p => p.Employee!.FullName)
                .ToListAsync();
        }

        public decimal CalculateTotalBasicSalary(List<Payroll> payrolls)
        {
            return payrolls.Sum(p => p.BasicSalary);
        }

        public decimal CalculateTotalBonus(List<Payroll> payrolls)
        {
            return payrolls.Sum(p => p.Bonus);
        }

        public decimal CalculateTotalDeduction(List<Payroll> payrolls)
        {
            return payrolls.Sum(p => p.Deduction);
        }

        public decimal CalculateTotalTax(List<Payroll> payrolls)
        {
            return payrolls.Sum(p => p.Tax);
        }

        public decimal CalculateTotalPayroll(List<Payroll> payrolls)
        {
            return payrolls.Sum(p => p.NetSalary);
        }
    }
}