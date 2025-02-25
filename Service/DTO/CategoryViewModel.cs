using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class CategoryViewModel
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(250)]
        public string CategoryDescription { get; set; }

        public int? ParentCategoryID { get; set; }
        public bool IsActive { get; set; }
        public string? ParentCategoryName { get; set; }
        public int NewsCount { get; set; }
    }
}
