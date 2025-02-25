using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace NMS.Controllers
{
    [Authorize(Roles = "1")] // Staff
    public class CategoryManagementController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryManagementController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string searchTerm, bool? activeOnly)
        {
            var viewModel = new CategorySearchViewModel
            {
                SearchTerm = searchTerm,
                ActiveOnly = activeOnly,
                Categories = await _categoryService.SearchCategoriesAsync(searchTerm, activeOnly)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.ParentCategories = await _categoryService.GetParentCategorySelectListAsync();
            return View(new CategoryViewModel { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategories = await _categoryService.GetParentCategorySelectListAsync();
                return View(model);
            }

            var result = await _categoryService.CreateCategoryAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.ParentCategories = await _categoryService.GetParentCategorySelectListAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync((short)id);
            if (category == null)
                return NotFound();

            ViewBag.ParentCategories = await _categoryService.GetParentCategorySelectListAsync();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentCategories = await _categoryService.GetParentCategorySelectListAsync();
                return View(model);
            }

            var result = await _categoryService.UpdateCategoryAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                ViewBag.ParentCategories = await _categoryService.GetParentCategorySelectListAsync();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync((short)id);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}

