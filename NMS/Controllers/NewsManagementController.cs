using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.DTO;
using Service.Interface;

namespace NMS.Controllers
{
    [Authorize(Roles = "1")] // Staff
    public class NewsManagementController : Controller
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IAccountService _accountService;

        public NewsManagementController(
            INewsArticleService newsService,
            ICategoryService categoryService,
            ITagService tagService,
            IAccountService accountService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(
            string searchTerm,
            int? categoryId,
            bool? activeOnly,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var categories = await _categoryService.GetAllCategoryAsync();

            var viewModel = new NewsArticleSearchViewModel
            {
                SearchTerm = searchTerm,
                CategoryID = categoryId,
                ActiveOnly = activeOnly,
                FromDate = fromDate,
                ToDate = toDate,
                Categories = new SelectList(categories, "CategoryId", "CategoryName"),
                NewsArticles = await _newsService.SearchAsync(
                    searchTerm, categoryId, activeOnly, fromDate, toDate)
            };

            return View(viewModel);
        }


        public async Task<IActionResult> MyNewsArticle(
            string searchTerm,
            int? categoryId,
            bool? activeOnly,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var categories = await _categoryService.GetAllCategoryAsync();
            var currentUser = await _accountService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var viewModel = new NewsArticleSearchViewModel
            {
                SearchTerm = searchTerm,
                CategoryID = categoryId,
                ActiveOnly = activeOnly,
                FromDate = fromDate,
                ToDate = toDate,
                Categories = new SelectList(categories, "CategoryId", "CategoryName"),
                NewsArticles = await _newsService.SearchMyNewsArticleAsync(
                    searchTerm, categoryId, activeOnly, fromDate, toDate, currentUser.AccountId)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            await PrepareViewBagForNewsForm();
            return View(new NewsArticleViewModel { NewsStatus = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsArticleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareViewBagForNewsForm();
                return View(model);
            }

            var currentUser = await _accountService.GetCurrentUserAsync();
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = currentUser.AccountId;

            var result = await _newsService.CreateNewsAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                await PrepareViewBagForNewsForm();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var news = await _newsService.GetByIdAsync(id);
            if (news == null)
                return NotFound();

            await PrepareViewBagForNewsForm();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(NewsArticleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PrepareViewBagForNewsForm();
                return View(model);
            }

            var currentUser = await _accountService.GetCurrentUserAsync();
            model.ModifiedDate = DateTime.Now;
            model.UpdateBy = currentUser.AccountId;

            var result = await _newsService.UpdateAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                await PrepareViewBagForNewsForm();
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _newsService.DeleteAsync(id);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        private async Task PrepareViewBagForNewsForm()
        {
            ViewBag.Categories = new SelectList(
                await _categoryService.GetActiveCategoriesActiveAsync(),
                "CategoryId",
                "CategoryName");

            ViewBag.Tags = new SelectList(
                await _tagService.GetAllTagsAsync(),
                "TagId",
                "TagName");
        }
    }
}
