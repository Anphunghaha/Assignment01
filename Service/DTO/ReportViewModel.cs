using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class ReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalArticles { get; set; }
        public int ActiveArticles { get; set; }
        public int InactiveArticles { get; set; }
        public List<NewsArticle> Articles { get; set; }
        public Dictionary<string, int> ArticlesByCategory { get; set; }
        public Dictionary<string, int> ArticlesByStaff { get; set; }
        public int CountTag { get; set; }
    }
}
