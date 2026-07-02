using ERP_version1.Data;
using ERP_version1.Models.HR;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public DepartmentService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<string?> CreateDepartmentAsync(Department department)
        {
            bool duplicateName = await _context.Departments
                .AnyAsync(d => d.DepartmentName == department.DepartmentName);

            if (duplicateName)
            {
                return "Department name already exists.";
            }

            department.CreatedAt = DateTime.Now;

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Create",
                "Department",
                "Departments",
                department.DepartmentId,
                $"Department created: {department.DepartmentName}."
            );

            return null;
        }

        public async Task<string?> UpdateDepartmentAsync(Department department)
        {
            bool duplicateName = await _context.Departments
                .AnyAsync(d => d.DepartmentName == department.DepartmentName &&
                               d.DepartmentId != department.DepartmentId);

            if (duplicateName)
            {
                return "Department name already exists.";
            }

            var existingDepartment = await _context.Departments
                .FindAsync(department.DepartmentId);

            if (existingDepartment == null)
            {
                return "Department not found.";
            }

            existingDepartment.DepartmentName = department.DepartmentName;
            existingDepartment.Description = department.Description;
            existingDepartment.IsActive = department.IsActive;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Update",
                "Department",
                "Departments",
                existingDepartment.DepartmentId,
                $"Department updated: {existingDepartment.DepartmentName}."
            );

            return null;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return false;
            }

            bool hasEmployees = await _context.Employees
                .AnyAsync(e => e.DepartmentId == id);

            bool hasDesignations = await _context.Designations
                .AnyAsync(d => d.DepartmentId == id);

            if (hasEmployees || hasDesignations)
            {
                department.IsActive = false;
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Deactivate",
                    "Department",
                    "Departments",
                    department.DepartmentId,
                    $"Department deactivated because related records exist: {department.DepartmentName}."
                );

                return true;
            }

            string departmentName = department.DepartmentName;

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Delete",
                "Department",
                "Departments",
                id,
                $"Department deleted: {departmentName}."
            );

            return true;
        }

        public bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.DepartmentId == id);
        }
    }
}