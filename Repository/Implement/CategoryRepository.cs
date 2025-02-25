using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implement
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FunewsManagementContext _context;

        public CategoryRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public Task<List<Category>> GetAllCategoryActiveAsync()
        {
            return _context.Categories.Where(c => c.IsActive ?? false).ToListAsync();
        }

        public Task<List<Category>> GetAllCategoryAsync()
        {
            return _context.Categories.ToListAsync();
        }

        public Task<Category?> GetCategoryByIdAsync(short id)
        {
            return _context.Categories.Where(c => c.CategoryId == id).Include(c => c.ParentCategory).Include(c => c.NewsArticles).Include(c => c.InverseParentCategory).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> SearchAsync(string searchTerm, bool? activeOnly)
        {
            var query = _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.NewsArticles)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c =>
                    c.CategoryName.Contains(searchTerm) ||
                    c.CategoryDesciption.Contains(searchTerm));
            }

            if (activeOnly.HasValue)
            {
                query = query.Where(c => c.IsActive == activeOnly.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetNewsCountAsync(short categoryId)
        {
            return await _context.NewsArticles
                .CountAsync(n => n.CategoryId == categoryId);
        }

        public async Task CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(short id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasNewsArticlesAsync(short id)
        {
            return await _context.NewsArticles
                .AnyAsync(n => n.CategoryId == id);
        }
    }
}
