using ERP_version1.Data;
using ERP_version1.Models.HR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class DesignationService : IDesignationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public DesignationService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<List<Designation>> GetAllDesignationsAsync()
        {
            return await _context.Designations
                .Include(d => d.Department)
                .OrderBy(d => d.Department!.DepartmentName)
                .ThenBy(d => d.DesignationName)
                .ToListAsync();
        }

        public async Task<Designation?> GetDesignationByIdAsync(int id)
        {
            return await _context.Designations
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DesignationId == id);
        }

        public async Task<string?> CreateDesignationAsync(Designation designation)
        {
            bool duplicateName = await _context.Designations.AnyAsync(d =>
                d.DesignationName == designation.DesignationName &&
                d.DepartmentId == designation.DepartmentId);

            if (duplicateName)
            {
                return "This designation already exists in the selected department.";
            }

            var department = await _context.Departments.FindAsync(designation.DepartmentId);

            if (department == null)
            {
                return "Department not found.";
            }

            designation.CreatedAt = DateTime.Now;

            _context.Designations.Add(designation);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Create",
                "Designation",
                "Designations",
                designation.DesignationId,
                $"Designation created: {designation.DesignationName} under {department.DepartmentName}."
            );

            return null;
        }

        public async Task<string?> UpdateDesignationAsync(Designation designation)
        {
            bool duplicateName = await _context.Designations.AnyAsync(d =>
                d.DesignationName == designation.DesignationName &&
                d.DepartmentId == designation.DepartmentId &&
                d.DesignationId != designation.DesignationId);

            if (duplicateName)
            {
                return "This designation already exists in the selected department.";
            }

            var existingDesignation = await _context.Designations
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DesignationId == designation.DesignationId);

            if (existingDesignation == null)
            {
                return "Designation not found.";
            }

            var department = await _context.Departments.FindAsync(designation.DepartmentId);

            if (department == null)
            {
                return "Department not found.";
            }

            existingDesignation.DesignationName = designation.DesignationName;
            existingDesignation.DepartmentId = designation.DepartmentId;
            existingDesignation.IsActive = designation.IsActive;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Update",
                "Designation",
                "Designations",
                existingDesignation.DesignationId,
                $"Designation updated: {existingDesignation.DesignationName} under {department.DepartmentName}."
            );

            return null;
        }

        public async Task<bool> DeleteDesignationAsync(int id)
        {
            var designation = await _context.Designations
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.DesignationId == id);

            if (designation == null)
            {
                return false;
            }

            bool hasEmployees = await _context.Employees
                .AnyAsync(e => e.DesignationId == id);

            if (hasEmployees)
            {
                designation.IsActive = false;
                await _context.SaveChangesAsync();

                await _auditLogService.LogAsync(
                    "Deactivate",
                    "Designation",
                    "Designations",
                    designation.DesignationId,
                    $"Designation deactivated because related employees exist: {designation.DesignationName}."
                );

                return true;
            }

            string designationName = designation.DesignationName;
            string departmentName = designation.Department?.DepartmentName ?? "Unknown Department";

            _context.Designations.Remove(designation);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Delete",
                "Designation",
                "Designations",
                id,
                $"Designation deleted: {designationName} from {departmentName}."
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

        public bool DesignationExists(int id)
        {
            return _context.Designations.Any(d => d.DesignationId == id);
        }
    }
}