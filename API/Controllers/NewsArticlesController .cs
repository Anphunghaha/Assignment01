using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Service.DTO;
using Service.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        // GET: api/NewsArticles
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _newsArticleService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/NewsArticles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _newsArticleService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/NewsArticles
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] NewsArticleViewModel model)
        {
            var result = await _newsArticleService.CreateNewsAsync(model);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // PUT: api/NewsArticles/{id}
        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] NewsArticleViewModel model)
        {
            var result = await _newsArticleService.UpdateAsync(model);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // DELETE: api/NewsArticles/{id}
        [Authorize(Policy = "RequireStaffRole")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _newsArticleService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // GET: api/NewsArticles/Search
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? searchTerm,
            [FromQuery] int? categoryId,
            [FromQuery] bool? activeOnly,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var result = await _newsArticleService.SearchAsync(searchTerm ?? "", categoryId, activeOnly, fromDate, toDate);
            return Ok(result);
        }

        // GET: api/NewsArticles/mine
        [HttpGet("mine")]
        [Authorize]
        public async Task<IActionResult> SearchMyNewsArticle(
            [FromQuery] string? searchTerm,
            [FromQuery] int? categoryId,
            [FromQuery] bool? activeOnly,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            // Giả sử bạn lấy accountId từ token
            var accountIdClaim = User.FindFirst("AccountId");
            if (accountIdClaim == null) return Unauthorized();
            var accountId = short.Parse(accountIdClaim.Value);

            var result = await _newsArticleService.SearchMyNewsArticleAsync(searchTerm ?? "", categoryId, activeOnly, fromDate, toDate, accountId);
            return Ok(result);
        }
    }
}
