using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace MonteCarlo.NET.Controllers
{
    public class LimitsController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly ILimitService _limits;

        public LimitsController(UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager, ILimitService limits)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _limits = limits;
        }

        [Authorize]
        public async Task<IActionResult> Limits()
        {
            var user = await _userManager.GetUserAsync(User);
            SetUserViewData(user);
            return View(await _limits.GetCurrentLimitAsync(user));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SetLimit(long limit)
        {
            var user = await _userManager.GetUserAsync(User);
            SetUserViewData(user);

            if (await _userManager.IsLockedOutAsync(user))
            {
                await _signInManager.SignOutAsync();
                TempData["ErrorMessage"] = "Twoje konto zostało zablokowane na 15 minut.";
                return RedirectToAction("Login");
            }

            var result = await _limits.SetLimitAsync(user, limit);
            if (result.Succeeded)
            {
                ViewData["Confirm"] = "Dodano nowy limit";
            }
            else
            {
                ViewData["Error"] = result.ErrorMessage;
            }

            return View("Limits", result.Limit);
        }

        private void SetUserViewData(UserAccount? user)
        {
            if (user != null)
            {
                ViewData["Level"] = user.Level;
                ViewData["Saldo"] = user.Balance;
            }
        }
    }
}
