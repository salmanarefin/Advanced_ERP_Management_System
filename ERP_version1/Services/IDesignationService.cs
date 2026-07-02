using ERP_version1.Models.HR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.Services
{
    public interface IDesignationService
    {
        Task<List<Designation>> GetAllDesignationsAsync();

        Task<Designation?> GetDesignationByIdAsync(int id);

        Task<string?> CreateDesignationAsync(Designation designation);

        Task<string?> UpdateDesignationAsync(Designation designation);

        Task<bool> DeleteDesignationAsync(int id);

        Task<SelectList> GetDepartmentSelectListAsync(object? selectedValue = null);

        bool DesignationExists(int id);
    }
}