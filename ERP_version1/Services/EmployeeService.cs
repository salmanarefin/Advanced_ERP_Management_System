using ERP_version1.Data;
using ERP_version1.Models.HR;
using ERP_version1.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public EmployeeService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<EmployeeIndexViewModel> GetEmployeesAsync(string? searchTerm, int page)
        {
            int pageSize = 5;

            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.EmployeeCode.Contains(searchTerm) ||
                    e.FullName.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm) ||
                    (e.Phone != null && e.Phone.Contains(searchTerm)) ||
                    (e.EmploymentStatus != null && e.EmploymentStatus.Contains(searchTerm)) ||
                    (e.Department != null && e.Department.DepartmentName.Contains(searchTerm)) ||
                    (e.Designation != null && e.Designation.DesignationName.Contains(searchTerm))
                );
            }

            int totalEmployees = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);

            if (page < 1)
            {
                page = 1;
            }

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var employees = await query
                .OrderBy(e => e.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new EmployeeIndexViewModel
            {
                Employees = employees,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalEmployees = totalEmployees
            };
        }

        public async Task<Employee?> GetEmployeeDetailsAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<Employee?> GetEmployeeForEditAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee?> GetEmployeeForDeleteAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<string?> CreateEmployeeAsync(Employee employee)
        {
            bool duplicateCode = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employee.EmployeeCode);

            if (duplicateCode)
            {
                return "Employee code already exists.";
            }

            bool duplicateEmail = await _context.Employees
                .AnyAsync(e => e.Email == employee.Email);

            if (duplicateEmail)
            {
                return "Employee email already exists.";
            }

            employee.CreatedAt = DateTime.Now;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Create",
                "Employee",
                "Employees",
                employee.EmployeeId,
                $"Employee created: {employee.FullName} ({employee.EmployeeCode})."
            );

            return null;
        }

        public async Task<string?> UpdateEmployeeAsync(Employee employee)
        {
            bool duplicateCode = await _context.Employees
                .AnyAsync(e => e.EmployeeCode == employee.EmployeeCode && e.EmployeeId != employee.EmployeeId);

            if (duplicateCode)
            {
                return "Employee code already exists.";
            }

            bool duplicateEmail = await _context.Employees
                .AnyAsync(e => e.Email == employee.Email && e.EmployeeId != employee.EmployeeId);

            if (duplicateEmail)
            {
                return "Employee email already exists.";
            }

            var existingEmployee = await _context.Employees.FindAsync(employee.EmployeeId);

            if (existingEmployee == null)
            {
                return "Employee not found.";
            }

            existingEmployee.EmployeeCode = employee.EmployeeCode;
            existingEmployee.FullName = employee.FullName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.Address = employee.Address;
            existingEmployee.JoiningDate = employee.JoiningDate;
            existingEmployee.BasicSalary = employee.BasicSalary;
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.DesignationId = employee.DesignationId;
            existingEmployee.EmploymentStatus = employee.EmploymentStatus;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Update",
                "Employee",
                "Employees",
                existingEmployee.EmployeeId,
                $"Employee updated: {existingEmployee.FullName} ({existingEmployee.EmployeeCode})."
            );

            return null;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return false;
            }

            bool hasAttendance = await _context.Attendances.AnyAsync(a => a.EmployeeId == id);
            bool hasLeave = await _context.LeaveApplications.AnyAsync(l => l.EmployeeId == id);
            bool hasPayroll = await _context.Payrolls.AnyAsync(p => p.EmployeeId == id);

            if (hasAttendance || hasLeave || hasPayroll)
            {
                employee.EmploymentStatus = "Inactive";
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Deactivate",
                    "Employee",
                    "Employees",
                    employee.EmployeeId,
                    $"Employee deactivated because related records exist: {employee.FullName}."
                );

                return true;
            }

            string employeeName = employee.FullName;
            string employeeCode = employee.EmployeeCode;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Delete",
                "Employee",
                "Employees",
                id,
                $"Employee deleted: {employeeName} ({employeeCode})."
            );

            return true;
        }

        public async Task<SelectList> GetDepartmentSelectListAsync(object? selectedValue = null)
        {
            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            return new SelectList(departments, "DepartmentId", "DepartmentName", selectedValue);
        }

        public async Task<SelectList> GetDesignationSelectListAsync(object? selectedValue = null)
        {
            var designations = await _context.Designations
                .Where(d => d.IsActive)
                .OrderBy(d => d.DesignationName)
                .ToListAsync();

            return new SelectList(designations, "DesignationId", "DesignationName", selectedValue);
        }

        public bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}