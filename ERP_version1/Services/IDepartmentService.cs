using ERP_version1.Models.HR;

namespace ERP_version1.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllDepartmentsAsync();

        Task<Department?> GetDepartmentByIdAsync(int id);

        Task<string?> CreateDepartmentAsync(Department department);

        Task<string?> UpdateDepartmentAsync(Department department);

        Task<bool> DeleteDepartmentAsync(int id);

        bool DepartmentExists(int id);
    }
}