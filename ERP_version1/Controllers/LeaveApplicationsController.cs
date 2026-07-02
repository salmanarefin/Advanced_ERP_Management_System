using ERP_version1.Data;
using ERP_version1.Models.LeaveModule;
using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    public class LeaveApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILeaveService _leaveService;

        public LeaveApplicationsController(ApplicationDbContext context, ILeaveService leaveService)
        {
            _context = context;
            _leaveService = leaveService;
        }

        public async Task<IActionResult> Index()
        {
            var leaveApplications = await _leaveService.GetAllLeaveApplicationsAsync();
            return View(leaveApplications);
        }

        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes.Where(l => l.IsActive), "LeaveTypeId", "LeaveTypeName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveApplication leaveApplication)
        {
            string? errorMessage = await _leaveService.ApplyLeaveAsync(leaveApplication);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Leave application submitted successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", errorMessage);

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", leaveApplication.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes.Where(l => l.IsActive), "LeaveTypeId", "LeaveTypeName", leaveApplication.LeaveTypeId);

            return View(leaveApplication);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _leaveService.GetLeaveApplicationDetailsAsync(id.Value);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            string? errorMessage = await _leaveService.ApproveLeaveAsync(id);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Leave application approved successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _leaveService.GetLeaveApplicationForActionAsync(id.Value);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectConfirmed(int id, string rejectionReason)
        {
            string? errorMessage = await _leaveService.RejectLeaveAsync(id, rejectionReason);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Leave application rejected successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = errorMessage;
            return RedirectToAction(nameof(Reject), new { id });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _leaveService.GetLeaveApplicationForDeleteAsync(id.Value);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await _leaveService.DeleteLeaveApplicationAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] = "Leave application deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Leave application not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}