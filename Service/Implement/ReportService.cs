using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Service.DTO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
    public class ReportService : IReportService
    {
        private readonly FunewsManagementContext _context;

        public ReportService(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<ReportViewModel> GenerateReportAsync(DateTime startDate, DateTime endDate)
        {
            var report = new ReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate
            };
            // Lấy danh sách bài viết trong khoảng thời gian
            var articles = await _context.NewsArticles
                .Where(article => article.CreatedDate >= startDate && article.CreatedDate <= endDate)
                .Include(article => article.Category)
                .Include(article => article.CreatedBy)
                .ToListAsync();

            report.TotalArticles = articles.Count;
            report.ActiveArticles = articles.Count(article => article.NewsStatus ?? false);
            report.InactiveArticles = articles.Count(article => !article.NewsStatus ?? false);

            // Thống kê theo danh mục
            report.ArticlesByCategory = articles
                .GroupBy(article => article.Category.CategoryName)
                .ToDictionary(g => g.Key, g => g.Count());

            // Thống kê theo Staff
            report.ArticlesByStaff = articles
                .GroupBy(article => article.CreatedBy.AccountName)
                .ToDictionary(g => g.Key, g => g.Count());

            // Thống kê tags
            var allTags = articles.SelectMany(article => article.Tags);
            report.CountTag = await _context.Tags.CountAsync();

            report.Articles = articles;

            return report;
        }
    }
}
