using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class NewsArticleDto
    {
        public string NewsArticleID { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? NewsContent { get; set; }

        public int? CategoryID { get; set; }
    }
}
