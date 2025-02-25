using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class AccountViewModel
    {
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(70)]
        public string AccountEmail { get; set; }

        public int? AccountRole { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,20}$",
                ErrorMessage = "Mật khẩu phải từ 8-20 ký tự. Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường và 1 ký tự đặc biệt.")]
        public string AccountPassword { get; set; }
    }
}
