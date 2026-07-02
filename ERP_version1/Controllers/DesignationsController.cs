using ERP_version1.Models.HR;
using ERP_version1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP_version1.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class DesignationsController : Controller
    {
        private readonly IDesignationService _designationService;

        public DesignationsController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        public async Task<IActionResult> Index()
        {
            var designations = await _designationService.GetAllDesignationsAsync();
            return View(designations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _designationService.GetDesignationByIdAsync(id.Value);

            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["DepartmentId"] = await _designationService.GetDepartmentSelectListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Designation designation)
        {
            if (ModelState.IsValid)
            {
                string? errorMessage = await _designationService.CreateDesignationAsync(designation);

                if (errorMessage == null)
                {
                    TempData["SuccessMessage"] = "Designation created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", errorMessage);
            }

            ViewData["DepartmentId"] = await _designationService.GetDepartmentSelectListAsync(designation.DepartmentId);
            return View(designation);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _designationService.GetDesignationByIdAsync(id.Value);

            if (designation == null)
            {
                return NotFound();
            }

            ViewData["DepartmentId"] = await _designationService.GetDepartmentSelectListAsync(designation.DepartmentId);
            return View(designation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Designation designation)
        {
            if (id != designation.DesignationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string? errorMessage = await _designationService.UpdateDesignationAsync(designation);

                if (errorMessage == null)
                {
                    TempData["SuccessMessage"] = "Designation updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", errorMessage);
            }

            ViewData["DepartmentId"] = await _designationService.GetDepartmentSelectListAsync(designation.DepartmentId);
            return View(designation);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _designationService.GetDesignationByIdAsync(id.Value);

            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool deleted = await _designationService.DeleteDesignationAsync(id);

            if (deleted)
            {
                TempData["SuccessMessage"] = "Designation deleted or deactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Designation not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}