using DataAccess.Model ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IAccountRepository
    {
        Task<SystemAccount?> GetByEmailAsync(string email);
        Task<SystemAccount?> GetByIdAsync(int id);
        Task<bool> ValidateCredentialsAsync(string email, string password);

        Task<IEnumerable<SystemAccount>> GetAllAsync();
        Task<IEnumerable<SystemAccount>> SearchAsync(string searchTerm, int? roleFilter);
        Task CreateAsync(SystemAccount account);
        Task UpdateAsync(SystemAccount account);
        Task DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}
