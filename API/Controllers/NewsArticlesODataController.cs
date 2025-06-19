using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Service.Interface;

namespace API.Controllers
{
    [Route("odata/NewsArticles")] // CHÍNH XÁC PHẢI TRÙNG TÊN EntitySet
    public class NewsArticlesODataController : ODataController
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesODataController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _newsArticleService.GetAllAsync();
            return Ok(result.AsQueryable());
        }

        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get([FromRoute] string key)
        {
            var item = await _newsArticleService.GetByIdAsync(key);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}
