using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class NewsArticleViewModel
    {
        public string? NewsArticleID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(400)]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [StringLength(150)]
        public string Headline { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string NewsContent { get; set; }

        public string NewsSource { get; set; }

        public int? CategoryID { get; set; }

        public bool NewsStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CategoryName { get; set; }
        public short CreatedBy { get; set; }
        public AccountViewModel? CreateByStaff { get; set; }
        public short UpdateBy { get; set; }
        public List<int> SelectedTags { get; set; } = new List<int>();
        public List<string> TagNames { get; set; } = new List<string>();
    }
}
