using ERP_version1.Data;
using ERP_version1.Models.PayrollModule;
using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin,PayrollOfficer")]
    public class PayrollsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayrollService _payrollService;

        public PayrollsController(ApplicationDbContext context, IPayrollService payrollService)
        {
            _context = context;
            _payrollService = payrollService;
        }

        public async Task<IActionResult> Index()
        {
            var payrolls = await _payrollService.GetAllPayrollsAsync();
            return View(payrolls);
        }

        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName");
            ViewBag.CurrentMonth = DateTime.Now.Month;
            ViewBag.CurrentYear = DateTime.Now.Year;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payroll payroll)
        {
            string? errorMessage = await _payrollService.GeneratePayrollAsync(payroll);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Payroll generated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", errorMessage);

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _payrollService.GetPayrollDetailsAsync(id.Value);

            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _payrollService.GetPayrollForDeleteAsync(id.Value);

            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await _payrollService.DeletePayrollAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] = "Payroll deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Payroll record not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}