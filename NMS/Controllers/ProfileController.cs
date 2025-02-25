using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace HE170291TranNamTruongMVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IAccountService _accountService;

        public ProfileController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _accountService.GetCurrentUserAsync();
            if(currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var profile = await _accountService.GetUserProfileAsync(currentUser.AccountId);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var result = await _accountService.UpdateProfileAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View("Index", model);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileViewModel model)
        {
            if (string.IsNullOrEmpty(model.CurrentPassword) ||
                string.IsNullOrEmpty(model.NewPassword) ||
                string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "All password fields are required");
                return RedirectToAction(nameof(Index));
            }

            var result = await _accountService.ChangePasswordAsync(
                model.AccountID,
                model.CurrentPassword,
                model.NewPassword);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
