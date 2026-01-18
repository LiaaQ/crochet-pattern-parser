using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Models;
using CrochetPatternParser.Data;

namespace CrochetPatternParser.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly SignInManager<ApplicationUserEntity> _signInManager;

        public AccountController(UserManager<ApplicationUserEntity> userManager, SignInManager<ApplicationUserEntity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUserEntity { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Pattern");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded) return RedirectToAction("Index", "Pattern");

            ModelState.AddModelError("", "Invalid login");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Pattern");
        }
    }
}
