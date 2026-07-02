using ERP_version1.Data;
using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    public class AttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAttendanceService _attendanceService;

        public AttendancesController(ApplicationDbContext context, IAttendanceService attendanceService)
        {
            _context = context;
            _attendanceService = attendanceService;
        }

        public async Task<IActionResult> Index()
        {
            var attendances = await _attendanceService.GetAllAttendancesAsync();
            return View(attendances);
        }

        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(int employeeId)
        {
            string? errorMessage = await _attendanceService.CheckInAsync(employeeId);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Check-in successful.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = errorMessage;
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int attendanceId)
        {
            string? errorMessage = await _attendanceService.CheckOutAsync(attendanceId);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Check-out successful.";
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _attendanceService.GetAttendanceDetailsAsync(id.Value);

            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _attendanceService.GetAttendanceForDeleteAsync(id.Value);

            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await _attendanceService.DeleteAttendanceAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] = "Attendance record deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Attendance record not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}