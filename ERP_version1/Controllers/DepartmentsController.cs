using ERP_version1.Models.HR;
using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return View(departments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetDepartmentByIdAsync(id.Value);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                string? errorMessage = await _departmentService.CreateDepartmentAsync(department);

                if (errorMessage == null)
                {
                    TempData["SuccessMessage"] = "Department created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", errorMessage);
            }

            return View(department);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetDepartmentByIdAsync(id.Value);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string? errorMessage = await _departmentService.UpdateDepartmentAsync(department);

                if (errorMessage == null)
                {
                    TempData["SuccessMessage"] = "Department updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", errorMessage);
            }

            return View(department);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _departmentService.GetDepartmentByIdAsync(id.Value);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await _departmentService.DeleteDepartmentAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] = "Department deleted or deactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Department not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}