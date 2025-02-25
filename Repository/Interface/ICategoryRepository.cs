using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoryActiveAsync();
        Task<List<Category>> GetAllCategoryAsync();
        Task<Category?> GetCategoryByIdAsync(short id);


        Task<IEnumerable<Category>> SearchAsync(string searchTerm, bool? activeOnly);
        Task<int> GetNewsCountAsync(short categoryId);
        Task CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(short id);
        Task<bool> HasNewsArticlesAsync(short id);
    }
}
