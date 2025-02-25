using DataAccess.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Repository.Interface;
using Service.DTO;
using Service.Helper;
using Service.Interface;
using System.Security.Claims;

namespace Service.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AdminConfiguration _adminConfig;

        public AccountService(IOptions<AdminConfiguration> adminConfig, IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor)
        {
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
            _adminConfig = adminConfig.Value;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            if (email == _adminConfig.Email && password == _adminConfig.Password)
            {
                //claim là attribute of user
                var adminClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _adminConfig.Name),
                new Claim(ClaimTypes.Email, _adminConfig.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };
                // Dai dien cho danh tinh cua nguoi dung 
                var identity1 = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal1 = new ClaimsPrincipal(identity1);

                // đăng nhập và lưu information with Cookie
                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal1);

                return true;
            }
            var isValid = await _accountRepository.ValidateCredentialsAsync(email, password);
            if (!isValid) return false;

            var user = await _accountRepository.GetByEmailAsync(email);
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.AccountName),
            new Claim(ClaimTypes.Email, user.AccountEmail),
            new Claim(ClaimTypes.Role, user.AccountRole.ToString())
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return true;
        }
        public async Task<SystemAccount?> GetCurrentUserAsync()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;
            return await _accountRepository.GetByEmailAsync(email);
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }



        public async Task<IEnumerable<AccountViewModel>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.Select(x => new AccountViewModel()
            {
                AccountEmail = x.AccountEmail,
                AccountID = x.AccountId,
                AccountName = x.AccountName,
                AccountPassword = x.AccountPassword,
                AccountRole = x.AccountRole
            });
        }

        public async Task<AccountViewModel?> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            return account != null ? new AccountViewModel
            {
                AccountEmail = account.AccountEmail,
                AccountID = account.AccountId,
                AccountName = account.AccountName,
                AccountPassword = account.AccountPassword,
                AccountRole = account.AccountRole
            } : null;
        }

        public async Task<IEnumerable<AccountViewModel>> SearchAccountsAsync(string searchTerm, int? roleFilter)
        {
            var accounts = await _accountRepository.SearchAsync(searchTerm, roleFilter);
            return accounts.Select(x => new AccountViewModel()
            {
                AccountEmail = x.AccountEmail,
                AccountID = x.AccountId,
                AccountName = x.AccountName,
                AccountPassword = x.AccountPassword,
                AccountRole = x.AccountRole
            });
        }

        public async Task<ServiceResult> CreateAccountAsync(AccountViewModel model)
        {
            if (await _accountRepository.EmailExistsAsync(model.AccountEmail))
            {
                return new ServiceResult { Success = false, Message = "Email already exists" };
            }
            var accounts = await GetAllAccountsAsync();
            short max = accounts.Max(a => a.AccountID);
            var account = new SystemAccount
            {
                AccountId = (short)(max + 1),
                AccountName = model.AccountName,
                AccountEmail = model.AccountEmail,
                AccountRole = model.AccountRole,
                AccountPassword = model.AccountPassword
            };

            await _accountRepository.CreateAsync(account);
            return new ServiceResult { Success = true, Message = "Account created successfully" };
        }

        public async Task<ServiceResult> UpdateAccountAsync(AccountViewModel model)
        {
            var account = await _accountRepository.GetByIdAsync(model.AccountID);
            if (account == null)
            {
                return new ServiceResult { Success = false, Message = "Account not found" };
            }

            account.AccountName = model.AccountName;
            account.AccountEmail = model.AccountEmail;
            account.AccountRole = model.AccountRole;
            if (model.AccountPassword != null)
            {
                account.AccountPassword = model.AccountPassword;
            }
            await _accountRepository.UpdateAsync(account);
            return new ServiceResult { Success = true, Message = "Account updated successfully" };
        }

        public async Task<ServiceResult> DeleteAccountAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return new ServiceResult { Success = false, Message = "Account not found" };
            }

            if (account.NewsArticles.Count() > 0)
            {
                return new ServiceResult { Success = false, Message = "Delete Account fail" };
            }
            await _accountRepository.DeleteAsync(id);
            return new ServiceResult { Success = true, Message = "Account deleted successfully" };
        }


        public async Task<ProfileViewModel?> GetUserProfileAsync(int accountId)
        {
            var user = await _accountRepository.GetByIdAsync(accountId);
            if (user == null)
                return null;

            return new ProfileViewModel
            {
                AccountID = user.AccountId,
                AccountName = user.AccountName,
                AccountEmail = user.AccountEmail,
                AccountRole = user.AccountRole,
            };
        }

        public async Task<ServiceResult> UpdateProfileAsync(ProfileViewModel model)
        {
            var user = await _accountRepository.GetByIdAsync(model.AccountID);
            if (user == null)
                return new ServiceResult { Success = false, Message = "User not found" };

            // Check if email is changed and already exists
            if (user.AccountEmail != model.AccountEmail)
            {
                if (await _accountRepository.EmailExistsAsync(model.AccountEmail))
                {
                    return new ServiceResult { Success = false, Message = "Email already exists" };
                }
            }

            user.AccountName = model.AccountName;
            user.AccountEmail = model.AccountEmail;

            await _accountRepository.UpdateAsync(user);
            await LogoutAsync();
            await LoginAsync(user.AccountEmail, user.AccountPassword);
            return new ServiceResult { Success = true, Message = "Profile updated successfully" };
        }

        public async Task<ServiceResult> ChangePasswordAsync(int accountId, string currentPassword, string newPassword)
        {
            var user = await _accountRepository.GetByIdAsync(accountId);
            if (user == null)
                return new ServiceResult { Success = false, Message = "User not found" };

            // Verify current password
            if (user.AccountPassword != currentPassword)
                return new ServiceResult { Success = false, Message = "Current password is incorrect" };

            // Update password
            user.AccountPassword = newPassword;
            await _accountRepository.UpdateAsync(user);

            return new ServiceResult { Success = true, Message = "Password changed successfully" };
        }


    }
}
