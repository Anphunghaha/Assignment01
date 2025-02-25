using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implement
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly FunewsManagementContext _context;

        public NewsArticleRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public Task<List<NewsArticle>> GetAllNewsArticlesActiveAsync()
        {
            return _context.NewsArticles.Where(n => n.NewsStatus ?? false).ToListAsync();
        }

        public Task<List<NewsArticle>> GetAllNewsArticlesAsync()
        {
            return _context.NewsArticles.ToListAsync();
        }

        public Task<NewsArticle?> GetNewsArticlesActiveByIdAsync(string id)
        {
            return _context.NewsArticles.Include(n => n.Category).Include(n => n.CreatedBy).Include(n => n.Tags).SingleOrDefaultAsync(n => (n.NewsStatus ?? false) && n.NewsArticleId.Equals(id));
        }

        public Task<List<NewsArticle>> GetNewsByCategoryAsync(short id)
        {
            return _context.NewsArticles.Where(n => (n.NewsStatus ?? false) && n.CategoryId == id).ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                    .ThenInclude(nt => nt.NewsArticles)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                    .ThenInclude(nt => nt.NewsArticles)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }

        public async Task<IEnumerable<NewsArticle>> SearchAsync(
            string searchTerm,
            int? categoryId,
            bool? activeOnly,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                    .ThenInclude(nt => nt.NewsArticles)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(n =>
                    n.NewsTitle.Contains(searchTerm) ||
                    n.Headline.Contains(searchTerm) ||
                    n.NewsContent.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId);
            }

            if (activeOnly.HasValue)
            {
                query = query.Where(n => n.NewsStatus == activeOnly.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= toDate.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task CreateAsync(NewsArticle newsArticle)
        {
            _context.NewsArticles.Add(newsArticle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NewsArticle newsArticle)
        {
            _context.NewsArticles.Update(newsArticle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var newsArticle = await GetByIdAsync(id);
            if (newsArticle != null)
            {
                using(var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        newsArticle.Tags.Clear();
                        await _context.SaveChangesAsync();
                        _context.NewsArticles.Remove(newsArticle);
                        await _context.SaveChangesAsync();
                        // Commit transaction
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        public async Task<string> GenerateNewsIdAsync()
        {
            var lastNews = await _context.NewsArticles
                .OrderByDescending(n => n.NewsArticleId)
                .FirstOrDefaultAsync();

            if (lastNews == null)
                return "1";

            if (int.TryParse(lastNews.NewsArticleId, out int lastId))
            {
                return (lastId + 1).ToString();
            }

            return Guid.NewGuid().ToString("N").Substring(0, 20);
        }

    }
}
