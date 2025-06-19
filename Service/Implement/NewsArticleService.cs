using DataAccess.Model;
using Repository.Implement;
using Repository.Interface;
using Service.DTO;
using Service.Helper;
using Service.Interface;

namespace Service.Implement
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository newsArticleRepository;
        private readonly ITagRepository tagRepository;
        private readonly IAccountRepository accountRepository;

        public NewsArticleService(IAccountRepository accountRepository, INewsArticleRepository newsArticleRepository, ITagRepository tagRepository)
        {
            this.newsArticleRepository = newsArticleRepository;
            this.tagRepository = tagRepository;
            this.accountRepository = accountRepository;
        }

        public Task<List<NewsArticle>> GetFeaturedNewsActiveAsync()
        {
            return newsArticleRepository.GetAllNewsArticlesActiveAsync();
        }

        public Task<List<NewsArticle>> GetNewsByCategoryAsync(short id)
        {
            return newsArticleRepository.GetNewsByCategoryAsync(id);
        }

        public async Task<NewsDetailViewModel?> GetNewsDetailAsync(string id)
        {
            var news = await newsArticleRepository.GetNewsArticlesActiveByIdAsync(id);
            if (news == null) { return null; }
            return new NewsDetailViewModel
            {
                CategoryName = news.Category.CategoryName,
                CreatedByName = news.CreatedBy.AccountName,
                CreatedDate = news.CreatedDate,
                Headline = news.Headline,
                NewsArticleID = news.NewsArticleId,
                NewsContent = news.NewsContent,
                NewsSource = news.NewsSource,
                NewsTitle = news.NewsTitle,
                Tags = news.Tags.Select(x => x.TagName).ToList(),
            };
        }


        public Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return newsArticleRepository.GetAllAsync();
        }


        public async Task<NewsArticleViewModel?> GetByIdAsync(string id)
        {
            var newsArticle = await newsArticleRepository.GetByIdAsync(id);
            if (newsArticle == null) { return null; }
            return new NewsArticleViewModel
            {
                CategoryID = (short)newsArticle.CreatedById,
                CategoryName = newsArticle.Category.CategoryName,
                CreatedBy = newsArticle.CreatedBy.AccountId,
                CreatedDate = newsArticle.CreatedDate ?? new DateTime(),
                Headline = newsArticle.Headline,
                ModifiedDate = newsArticle.ModifiedDate,
                NewsArticleID = newsArticle.NewsArticleId,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                NewsStatus = newsArticle.NewsStatus ?? false,
                NewsTitle = newsArticle.NewsTitle,
                SelectedTags = newsArticle.Tags.Select(x => x.TagId).ToList(),
                TagNames = newsArticle.Tags.Select(x => x.TagName).ToList()
            };
        }

        public async Task<IEnumerable<NewsArticleViewModel>> SearchAsync(
            string searchTerm,
            int? categoryId,
            bool? activeOnly,
            DateTime? fromDate,
            DateTime? toDate)
        {

            var news = await newsArticleRepository.SearchAsync(searchTerm, categoryId, activeOnly, fromDate, toDate);
            return news.Select(n => new NewsArticleViewModel
            {
                CategoryID = (short)n.CreatedById,
                CategoryName = n.Category.CategoryName,
                CreatedBy = n.CreatedBy.AccountId,
                CreatedDate = n.CreatedDate ?? new DateTime(),
                Headline = n.Headline,
                ModifiedDate = n.ModifiedDate,
                NewsArticleID = n.NewsArticleId,
                NewsContent = n.NewsContent,
                NewsSource = n.NewsSource,
                NewsStatus = n.NewsStatus ?? false,
                NewsTitle = n.NewsTitle,
                SelectedTags = n.Tags.Select(x => x.TagId).ToList(),
                TagNames = n.Tags.Select(x => x.TagName).ToList(),
                UpdateBy = n.CreatedBy.AccountId,
                CreateByStaff = new AccountViewModel { AccountName = n.CreatedBy?.AccountName}
            });
        }

        public async Task<ServiceResult> CreateNewsAsync(NewsArticleViewModel model)
        {
            try
            {
                var newsArticle = new NewsArticle();
                newsArticle.NewsArticleId = await newsArticleRepository.GenerateNewsIdAsync();
                newsArticle.Headline = model.Headline;
                newsArticle.NewsSource = model.NewsSource;
                newsArticle.CategoryId = (short)model.CategoryID;
                newsArticle.CreatedById = model.CreatedBy;
                newsArticle.NewsTitle = model.NewsTitle;
                newsArticle.CreatedDate = model.CreatedDate;
                newsArticle.NewsContent = model.NewsContent;
                newsArticle.NewsStatus = model.NewsStatus;
                newsArticle.ModifiedDate = model.CreatedDate;
                newsArticle.UpdatedById = model.CreatedBy;
                var tags = await tagRepository.GetAllTagsAsync();
                var tagsSelect = tags.Where(t => model.SelectedTags.Contains(t.TagId)).ToList();
                newsArticle.Tags = tagsSelect;
                await newsArticleRepository.CreateAsync(newsArticle);
                return new ServiceResult
                {
                    Success = true,
                    Message = "Create news article successful"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult> UpdateAsync(NewsArticleViewModel model)
        {
            try
            {
                if (model.NewsArticleID == null)
                {
                    return new ServiceResult { Success = false, Message = "NewsArticleID not null" };
                }
                var newsArticle = await newsArticleRepository.GetByIdAsync(model.NewsArticleID);
                if (newsArticle == null)
                {
                    return new ServiceResult { Success = false, Message = "Not found" };
                }

                newsArticle.Headline = model.Headline;
                newsArticle.NewsSource = model.NewsSource;
                if (model.CategoryID.HasValue)
                {
                    newsArticle.CategoryId = (short)model.CategoryID;
                }
                newsArticle.NewsTitle = model.NewsTitle;
                newsArticle.NewsContent = model.NewsContent;
                newsArticle.NewsStatus = model.NewsStatus;
                newsArticle.ModifiedDate = model.ModifiedDate;
                newsArticle.UpdatedById = model.CreatedBy;
                var tags = await tagRepository.GetAllTagsAsync();
                var tagsSelect = tags.Where(t => model.SelectedTags.Contains(t.TagId)).ToList();
                newsArticle.Tags.Clear();
                newsArticle.Tags = tagsSelect;
                await newsArticleRepository.UpdateAsync(newsArticle);
                return new ServiceResult
                {
                    Success = true,
                    Message = "Update news article successful"
                };
            }catch (Exception ex)
            {
                return new ServiceResult { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResult> DeleteAsync(string id)
        {
            try
            {
                var newsArticle = await newsArticleRepository.GetByIdAsync(id);
                if (newsArticle == null)
                {
                    return new ServiceResult { Success = false, Message = "Not found" };
                }
                await newsArticleRepository.DeleteAsync(id);
                return new ServiceResult
                {
                    Success = true,
                    Message = "Delete news article successful"
                };
            }catch(Exception ex)
            {
                return new ServiceResult { Success = false, Message = ex.Message };
            }
        }

        public Task<string> GenerateNewsIdAsync()
        {
            return newsArticleRepository.GenerateNewsIdAsync();
        }

        public async Task<IEnumerable<NewsArticleViewModel>> SearchMyNewsArticleAsync(string searchTerm, int? categoryId, bool? activeOnly, DateTime? fromDate, DateTime? toDate, short accountId)
        {
            var news = await SearchAsync(searchTerm, categoryId, activeOnly, fromDate, toDate);
            return news.Where(n => n.CreatedBy == accountId).ToList();
        }

        public async Task<NewsDetailViewModel?> GetNewsArticleByIdAsync(string id)
        {
            var news = await newsArticleRepository.GetByIdAsync(id);
            if (news == null)
                return null;
            SystemAccount? updateBy = null;
            if(news.UpdatedById != null)
            {
                updateBy = await accountRepository.GetByIdAsync((int)news.UpdatedById);
            }

            return new NewsDetailViewModel
            {
                NewsArticleID = news.NewsArticleId,
                NewsTitle = news.NewsTitle,
                Headline = news.Headline,
                CreatedDate = news.CreatedDate,
                NewsContent = news.NewsContent,
                NewsSource = news.NewsSource,
                CategoryName = news.Category.CategoryName,
                CreatedByName = news.CreatedBy.AccountName,
                Tags = news.Tags.Select(x => x.TagName).ToList(),
                NewsStatus = news.NewsStatus,
                UpdateByName = updateBy != null ? updateBy.AccountName: null,
                ModifiedDate = news.ModifiedDate
            };
        }


        }
    }

