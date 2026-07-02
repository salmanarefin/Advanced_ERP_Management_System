using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            var model = await _auditLogService.GetAuditLogsAsync(searchTerm, page);
            return View(model);
        }
    }
}