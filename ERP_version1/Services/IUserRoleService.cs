using ERP_version1.ViewModels;

namespace ERP_version1.Services
{
    public interface IUserRoleService
    {
        Task<List<UserRolesViewModel>> GetAllUsersWithRolesAsync();

        Task<ManageUserRolesViewModel?> GetManageUserRolesAsync(string userId);

        Task<string?> AddRoleToUserAsync(string userId, string selectedRole);

        Task<string?> RemoveRoleFromUserAsync(string userId, string roleName);
    }
}