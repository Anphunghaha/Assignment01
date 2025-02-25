using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class NewsArticleSearchViewModel
    {
        public string SearchTerm { get; set; }
        public int? CategoryID { get; set; }
        public bool? ActiveOnly { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IEnumerable<NewsArticleViewModel> NewsArticles { get; set; }
        public SelectList Categories { get; set; }
    }
}
