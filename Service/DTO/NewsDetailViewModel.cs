using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class NewsDetailViewModel
    {
        public string NewsArticleID { get; set; }
        public string NewsTitle { get; set; }
        public string Headline { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string NewsContent { get; set; }
        public string NewsSource { get; set; }
        public string CategoryName { get; set; }
        public string CreatedByName { get; set; }
        public List<string> Tags { get; set; }
        public bool? NewsStatus { get; set; }
        public string? UpdateByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
