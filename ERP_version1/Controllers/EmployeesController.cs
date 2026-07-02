using ERP_version1.Models.HR;
using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            var model = await _employeeService.GetEmployeesAsync(searchTerm, page);
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeDetailsAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["DepartmentId"] = await _employeeService.GetDepartmentSelectListAsync();
            ViewData["DesignationId"] = await _employeeService.GetDesignationSelectListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                string? errorMessage = await _employeeService.CreateEmployeeAsync(employee);

                if (errorMessage == null)
                {
                    TempData["SuccessMessage"] = "Employee created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", errorMessage);
            }

            ViewData["DepartmentId"] = await _employeeService.GetDepartmentSelectListAsync(employee.DepartmentId);
            ViewData["DesignationId"] = await _employeeService.GetDesignationSelectListAsync(employee.DesignationId);

            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeForEditAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            ViewData["DepartmentId"] = await _employeeService.GetDepartmentSelectListAsync(employee.DepartmentId);
            ViewData["DesignationId"] = await _employeeService.GetDesignationSelectListAsync(employee.DesignationId);

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string? errorMessage = await _employeeService.UpdateEmployeeAsync(employee);

                if (errorMessage == null)
                {
                    TempData["SuccessMessage"] = "Employee updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", errorMessage);
            }

            ViewData["DepartmentId"] = await _employeeService.GetDepartmentSelectListAsync(employee.DepartmentId);
            ViewData["DesignationId"] = await _employeeService.GetDesignationSelectListAsync(employee.DesignationId);

            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeForDeleteAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await _employeeService.DeleteEmployeeAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] = "Employee deleted or deactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Employee not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}