using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class CategorySearchViewModel
    {
        public string SearchTerm { get; set; }
        public bool? ActiveOnly { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
    }
}
