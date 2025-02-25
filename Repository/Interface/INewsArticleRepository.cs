using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface INewsArticleRepository
    {
        Task<List<NewsArticle>> GetAllNewsArticlesAsync();
        Task<List<NewsArticle>> GetAllNewsArticlesActiveAsync();
        Task<NewsArticle?> GetNewsArticlesActiveByIdAsync(string id);
        Task<List<NewsArticle>> GetNewsByCategoryAsync(short id);

        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<NewsArticle?> GetByIdAsync(string id);
        Task<IEnumerable<NewsArticle>> SearchAsync(
            string searchTerm,
            int? categoryId,
            bool? activeOnly,
            DateTime? fromDate,
            DateTime? toDate);
        Task CreateAsync(NewsArticle newsArticle);
        Task UpdateAsync(NewsArticle newsArticle);
        Task DeleteAsync(string id);
        Task<string> GenerateNewsIdAsync();
    }
}
