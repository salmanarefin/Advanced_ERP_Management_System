using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _userRoleService.GetAllUsersWithRolesAsync();
            return View(model);
        }

        public async Task<IActionResult> Manage(string userId)
        {
            var model = await _userRoleService.GetManageUserRolesAsync(userId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(string userId, string selectedRole)
        {
            string? errorMessage = await _userRoleService.AddRoleToUserAsync(userId, selectedRole);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Role added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction(nameof(Manage), new { userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            string? errorMessage = await _userRoleService.RemoveRoleFromUserAsync(userId, roleName);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Role removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction(nameof(Manage), new { userId });
        }
    }
}