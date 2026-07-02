using ERP_version1.Models.AttendanceModule;
using ERP_version1.Models.HR;
using ERP_version1.Models.LeaveModule;
using ERP_version1.Models.PayrollModule;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.Services
{
    public interface IReportService
    {
        Task<List<Employee>> GetEmployeeReportAsync(
            int? departmentId,
            string? status,
            string? searchTerm);

        Task<SelectList> GetDepartmentSelectListAsync(object? selectedValue = null);

        Task<SelectList> GetEmployeeSelectListAsync(object? selectedValue = null);

        Task<SelectList> GetLeaveTypeSelectListAsync(object? selectedValue = null);

        Task<List<Attendance>> GetAttendanceReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            int? employeeId,
            string? status,
            string? searchTerm);

        Task<List<LeaveApplication>> GetLeaveReportAsync(
            DateTime? fromDate,
            DateTime? toDate,
            int? employeeId,
            int? leaveTypeId,
            string? status,
            string? searchTerm);

        Task<List<Payroll>> GetPayrollReportAsync(
            int? month,
            int? year,
            int? employeeId,
            string? searchTerm);

        decimal CalculateTotalBasicSalary(List<Payroll> payrolls);

        decimal CalculateTotalBonus(List<Payroll> payrolls);

        decimal CalculateTotalDeduction(List<Payroll> payrolls);

        decimal CalculateTotalTax(List<Payroll> payrolls);

        decimal CalculateTotalPayroll(List<Payroll> payrolls);
    }
}