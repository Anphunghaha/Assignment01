using DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class NewsByCategoryModel
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<NewsArticle> Articles { get; set; }
    }
}
