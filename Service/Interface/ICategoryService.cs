using Service.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Helper;
using DataAccess.Model;

namespace Service.Interface
{
    public interface ICategoryService
    {
        Task<List<Category>> GetActiveCategoriesActiveAsync();
        Task<List<Category>> GetAllCategoryAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(short id);
        Task<List<CategoryViewModel>> SearchCategoriesAsync(string searchTerm, bool? activeOnly);
        Task<List<SelectListItem>> GetParentCategorySelectListAsync();
        Task<ServiceResult> CreateCategoryAsync(CategoryViewModel category);
        Task<ServiceResult> UpdateCategoryAsync(CategoryViewModel category);
        Task<ServiceResult> DeleteCategoryAsync(short id);
    }
}
