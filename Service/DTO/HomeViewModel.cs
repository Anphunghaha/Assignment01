using DataAccess.Model;

namespace Service.DTO
{
    public class HomeViewModel
    {
        public List<NewsArticle> FeaturedNews { get; set; }
        public List<Category> Categories { get; set; }
    }
}
