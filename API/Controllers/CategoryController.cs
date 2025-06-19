using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoryAsync();
            var result = categories.Select(x => new CategoryViewModel
            {
                CategoryID = x.CategoryId,
                CategoryName = x.CategoryName,
                CategoryDescription = x.CategoryDesciption,
                IsActive = x.IsActive ?? false,
                ParentCategoryID = x.ParentCategoryId,
                ParentCategoryName = x?.ParentCategory?.CategoryName
            }).ToList();

            return Ok(result);
        }

        // GET: api/Category/active
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _categoryService.GetActiveCategoriesActiveAsync();
            return Ok(result.Select(x => new CategoryViewModel
            {
                CategoryID = x.CategoryId,
                CategoryName = x.CategoryName
            }));
        }

        // GET: api/Category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateCategoryAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(short id, [FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.CategoryID = id;
            var result = await _categoryService.UpdateCategoryAsync(model);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/Category/search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm, [FromQuery] bool? activeOnly)
        {
            var result = await _categoryService.SearchCategoriesAsync(searchTerm, activeOnly);
            return Ok(result);
        }

        // GET: api/Category/parents
        [HttpGet("parents")]
        public async Task<IActionResult> GetParentCategories()
        {
            var result = await _categoryService.GetParentCategorySelectListAsync();
            return Ok(result);
        }
    }
}
