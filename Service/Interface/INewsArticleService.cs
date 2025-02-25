using DataAccess.Model;
using Service.DTO;
using Service.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface INewsArticleService
    {
        Task<List<NewsArticle>> GetFeaturedNewsActiveAsync();
        Task<NewsDetailViewModel?> GetNewsDetailAsync(string id);
        Task<List<NewsArticle>> GetNewsByCategoryAsync(short id);

        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<NewsArticleViewModel?> GetByIdAsync(string id);
        Task<IEnumerable<NewsArticleViewModel>> SearchAsync(
            string searchTerm,
            int? categoryId,
            bool? activeOnly,
            DateTime? fromDate,
            DateTime? toDate);
        Task<ServiceResult> CreateNewsAsync(NewsArticleViewModel model);
        Task<ServiceResult> UpdateAsync(NewsArticleViewModel model);
        Task<ServiceResult> DeleteAsync(string id);
        Task<string> GenerateNewsIdAsync();
        Task<IEnumerable<NewsArticleViewModel>> SearchMyNewsArticleAsync(string searchTerm, int? categoryId, bool? activeOnly, DateTime? fromDate, DateTime? toDate, short accountId);
        Task<NewsDetailViewModel?> GetNewsArticleByIdAsync(string id);
    }
}
