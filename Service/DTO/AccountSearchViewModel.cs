using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class AccountSearchViewModel
    {
        public string SearchTerm { get; set; }
        public int? RoleFilter { get; set; }
        public IEnumerable<AccountViewModel> Accounts { get; set; }
    }
}
