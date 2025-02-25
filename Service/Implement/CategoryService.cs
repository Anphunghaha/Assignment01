using DataAccess.Model;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repository.Interface;
using Service.DTO;
using Service.Helper;
using Service.Interface;

namespace Service.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResult> CreateCategoryAsync(CategoryViewModel category)
        {
            var categories = await _repository.GetAllCategoryAsync();
            if(categories.Any(c => c.CategoryName.Equals(category.CategoryName))){
                return new ServiceResult { Success = false, Message = "Category is exist" };
            }
            var cate = new Category {
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDescription,
                IsActive = category.IsActive,
            };
            if (category.ParentCategoryID.HasValue)
            {
                cate.ParentCategoryId = (short)category.ParentCategoryID;
            }
            await _repository.CreateAsync(cate);
            return new ServiceResult { Success = true, Message = "Create Successful" };
        }

        public async Task<ServiceResult> DeleteCategoryAsync(short id)
        {
            var category = await _repository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return new ServiceResult { Success = false, Message = "Category not found" };
            }
            if(category.NewsArticles.Count > 0 ||  category.InverseParentCategory.Count > 0 && category.InverseParentCategory.Any(c => c.CategoryId != id))
            {
                return new ServiceResult { Success = false, Message = "Category cant not delete" };
            }
            await _repository.DeleteAsync(id);
            return new ServiceResult { Success = true, Message = "Delete category successful" };
        }

        public Task<List<Category>> GetActiveCategoriesActiveAsync()
        {
            return _repository.GetAllCategoryActiveAsync();
        }

        public Task<List<Category>> GetAllCategoryAsync()
        {
            return _repository.GetAllCategoryAsync();
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(short id)
        {
            var category = await _repository.GetCategoryByIdAsync(id);
            if(category == null) {
                return null;
            }
            return  new CategoryViewModel
            {
                CategoryDescription = category.CategoryDesciption,
                CategoryName = category.CategoryName,
                CategoryID = category.CategoryId,
                IsActive = category.IsActive ?? false,
                NewsCount = category.NewsArticles.Count(),
                ParentCategoryID = category.ParentCategoryId,
                ParentCategoryName = category?.ParentCategory?.CategoryName
            };
        }

        public async Task<List<SelectListItem>> GetParentCategorySelectListAsync()
        {
            var categories = await _repository.GetAllCategoryAsync();
            return categories.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString(),
            }).ToList();
        }

        public async Task<List<CategoryViewModel>> SearchCategoriesAsync(string searchTerm, bool? activeOnly)
        {
            var categories = await _repository.SearchAsync(searchTerm, activeOnly);
            return categories.Select(x => new CategoryViewModel
            {
                CategoryDescription =  x.CategoryDesciption,
                CategoryName = x.CategoryName,
                CategoryID = x.CategoryId,
                IsActive = x.IsActive ?? false,
                NewsCount = x.NewsArticles.Count(),
                ParentCategoryID = x.ParentCategoryId,
                ParentCategoryName = x?.ParentCategory?.CategoryName
            }).ToList();
        }

        public async Task<ServiceResult> UpdateCategoryAsync(CategoryViewModel model)
        {
            if(model.CategoryID != 0)
            {
                var category = await _repository.GetCategoryByIdAsync((short)model.CategoryID);
                if (category == null)
                {
                    return new ServiceResult { Success = false, Message = "Category not found" };
                }

                category.CategoryName = model.CategoryName;
                if(model.ParentCategoryID.HasValue)
                {
                    category.ParentCategoryId = (short)model.ParentCategoryID;
                }
                category.CategoryDesciption = model.CategoryDescription;
                category.IsActive = model.IsActive;

                await _repository.UpdateAsync(category);
                return new ServiceResult { Success = true, Message = "Account updated successfully" };
            }
            return new ServiceResult { Success = false, Message = "Account not found" };
        }
    }
}
