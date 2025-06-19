using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var report = await _reportService.GenerateReportAsync(startDate, endDate);
            if (report == null)
                return NotFound(new { message = "No report data found" });
            return Ok(report);
        }
    }
}
