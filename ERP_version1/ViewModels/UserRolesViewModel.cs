using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public IList<string> UserRoles { get; set; } = new List<string>();

        public string? SelectedRole { get; set; }

        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();
    }
}