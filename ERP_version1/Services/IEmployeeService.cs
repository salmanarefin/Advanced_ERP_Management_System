using ERP_version1.Models.HR;
using ERP_version1.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeIndexViewModel> GetEmployeesAsync(string? searchTerm, int page);

        Task<Employee?> GetEmployeeDetailsAsync(int id);

        Task<Employee?> GetEmployeeForEditAsync(int id);

        Task<Employee?> GetEmployeeForDeleteAsync(int id);

        Task<string?> CreateEmployeeAsync(Employee employee);

        Task<string?> UpdateEmployeeAsync(Employee employee);

        Task<bool> DeleteEmployeeAsync(int id);

        Task<SelectList> GetDepartmentSelectListAsync(object? selectedValue = null);

        Task<SelectList> GetDesignationSelectListAsync(object? selectedValue = null);

        bool EmployeeExists(int id);
    }
}