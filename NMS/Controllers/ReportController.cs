using Microsoft.AspNetCore.Mvc;
using Service.Interface;

namespace NMS.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GenerateReport(DateTime startDate, DateTime endDate)
        {
            var reportData = await _reportService.GenerateReportAsync(startDate, endDate);
            return PartialView("_ReportPartial", reportData);
        }
    }
}
