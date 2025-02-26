using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace NMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountManagementController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountManagementController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(string searchTerm, int? roleFilter, string sortOrder)
        {
            var accounts = await _accountService.SearchAccountsAsync(searchTerm, roleFilter);

            // Sắp xếp theo sortOrder
            accounts = sortOrder switch
            {
                "name_desc" => accounts.OrderByDescending(a => a.AccountName).ToList(),
                "email_asc" => accounts.OrderBy(a => a.AccountEmail).ToList(),
                "email_desc" => accounts.OrderByDescending(a => a.AccountEmail).ToList(),
                _ => accounts.OrderBy(a => a.AccountName).ToList(), // Mặc định sắp xếp theo Name (A-Z)
            };

            var viewModel = new AccountSearchViewModel
            {
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                SortOrder = sortOrder,
                Accounts = accounts
            };

            return View(viewModel);
        }


        public IActionResult Create()
        {
            return View(new AccountViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.CreateAccountAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.UpdateAccountAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accountService.DeleteAccountAsync(id);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
