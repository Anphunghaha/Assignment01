using Microsoft.AspNetCore.Mvc;
using NMS.Models;
using Service.DTO;
using Service.Interface;
using System.Diagnostics;

namespace NMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public HomeController(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(short? id)
        {
            var viewModel = new HomeViewModel
            {
                FeaturedNews = await _newsService.GetFeaturedNewsActiveAsync(),
                Categories = await _categoryService.GetActiveCategoriesActiveAsync(),
            };
            if (id.HasValue)
            {
                viewModel.FeaturedNews = await _newsService.GetNewsByCategoryAsync((short)id);
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }


    }
}
