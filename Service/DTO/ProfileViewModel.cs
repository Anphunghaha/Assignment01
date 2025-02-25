using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class ProfileViewModel
    {
        public int AccountID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [StringLength(70)]
        public string AccountEmail { get; set; }

        [StringLength(70)]
        public string? CurrentPassword { get; set; }

        [StringLength(70, MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        public int? AccountRole { get; set; }
        public string RoleName => AccountRole == 1 ? "Staff" : "Lecturer";
        public int NewsCount { get; set; }
    }
}
