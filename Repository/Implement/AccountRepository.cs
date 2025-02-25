using DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implement
{
    public class AccountRepository : IAccountRepository
    {
        private readonly FunewsManagementContext _context;

        public AccountRepository(FunewsManagementContext context)
        {
            _context = context;
        }

        public async Task<SystemAccount?> GetByEmailAsync(string email)
        {
            return await _context.SystemAccounts
                .FirstOrDefaultAsync(x => x.AccountEmail == email);
        }

        public async Task<SystemAccount?> GetByIdAsync(int id)
        {
            return await _context.SystemAccounts.Include(a => a.NewsArticles)
                .FirstOrDefaultAsync(x => x.AccountId == id);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var account = await GetByEmailAsync(email);
            if (account == null) return false;
            return account.AccountPassword == password; // Note: In production, use proper password hashing
        }



        public async Task<IEnumerable<SystemAccount>> GetAllAsync()
        {
            return await _context.SystemAccounts.ToListAsync();
        }

        public async Task<IEnumerable<SystemAccount>> SearchAsync(string searchTerm, int? roleFilter)
        {
            var query = _context.SystemAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x =>
                    x.AccountName.Contains(searchTerm) ||
                    x.AccountEmail.Contains(searchTerm));
            }

            if (roleFilter.HasValue)
            {
                query = query.Where(x => x.AccountRole == roleFilter);
            }

            return await query.ToListAsync();
        }

        public async Task CreateAsync(SystemAccount account)
        {
            _context.SystemAccounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SystemAccount account)
        {
            _context.SystemAccounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var account = await GetByIdAsync(id);
            if (account != null)
            {
                _context.SystemAccounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.SystemAccounts
                .AnyAsync(x => x.AccountEmail == email);
        }
    }
}
