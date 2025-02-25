using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace NMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
          //  ModelState kiểm tra xem dữ liệu đầu vào(model) có hợp lệ không
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.LoginAsync(model.Email, model.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Email or Password is Invalid.");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index","Home");
        }

    }
}
