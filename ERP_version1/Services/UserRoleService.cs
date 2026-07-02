using ERP_version1.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP_version1.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditLogService _auditLogService;

        public UserRoleService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuditLogService auditLogService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditLogService = auditLogService;
        }

        public async Task<List<UserRolesViewModel>> GetAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? "No Email",
                    Roles = roles
                });
            }

            return model;
        }

        public async Task<ManageUserRolesViewModel?> GetManageUserRolesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var allRoles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? "No Email",
                UserRoles = userRoles,
                AllRoles = allRoles.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name
                }).ToList()
            };

            return model;
        }

        public async Task<string?> AddRoleToUserAsync(string userId, string selectedRole)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(selectedRole))
            {
                return "Please select a valid role.";
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return "User not found.";
            }

            bool roleExists = await _roleManager.RoleExistsAsync(selectedRole);

            if (!roleExists)
            {
                return "Selected role does not exist.";
            }

            bool alreadyInRole = await _userManager.IsInRoleAsync(user, selectedRole);

            if (alreadyInRole)
            {
                return "User already has this role.";
            }

            var result = await _userManager.AddToRoleAsync(user, selectedRole);

            if (!result.Succeeded)
            {
                return "Could not add role.";
            }

            await _auditLogService.LogAsync(
                "Add Role",
                "User Role",
                "AspNetUserRoles",
                null,
                $"Role {selectedRole} added to user {user.Email}."
            );

            return null;
        }

        public async Task<string?> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleName))
            {
                return "Invalid request.";
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return "User not found.";
            }

            if (user.Email == "admin@gmail.com" && roleName == "Admin")
            {
                return "You cannot remove Admin role from the default admin account.";
            }

            bool isInRole = await _userManager.IsInRoleAsync(user, roleName);

            if (!isInRole)
            {
                return "User does not have this role.";
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                return "Could not remove role.";
            }

            await _auditLogService.LogAsync(
                "Remove Role",
                "User Role",
                "AspNetUserRoles",
                null,
                $"Role {roleName} removed from user {user.Email}."
            );

            return null;
        }
    }
}