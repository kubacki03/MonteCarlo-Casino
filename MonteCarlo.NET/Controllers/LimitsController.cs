using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Controllers
{
    public class LimitsController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly CasinoContext _context;
        private readonly SignInManager<UserAccount> _signInManager;
        public LimitsController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, CasinoContext context, SignInManager<UserAccount> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        [Authorize]
        public async Task<IActionResult> Limits()
        {
            var user = await _userManager.GetUserAsync(User);
            var currentLimit = _context.Limits
            .Where(l => l.UserAccountId == user.Id)
            .AsEnumerable()
            .OrderBy(l => Math.Abs((l.Date - DateTime.Now).Ticks))
            .FirstOrDefault();

            if (user != null)
            {
                ViewData["Level"] = user.Level;
                ViewData["Saldo"] = user.Balance;
            }
            return View(currentLimit);
        }


        [HttpPost]
        public async Task<IActionResult> SetLimit(long limit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }
            if (user != null && await _userManager.IsLockedOutAsync(user))
            {

                await _signInManager.SignOutAsync();
                TempData["ErrorMessage"] = "Twoje konto zostało zablokowane na 15 minut.";
                return RedirectToAction("Login");
            }

            var currentLimit = _context.Limits
         .Where(l => l.UserAccountId == user.Id)
         .AsEnumerable()
         .OrderBy(l => Math.Abs((l.Date - DateTime.Now).Ticks))
         .FirstOrDefault();


            if (currentLimit != null && DateTime.Now < currentLimit.Date.AddDays(1))
            {
                ViewData["Error"] = "Nie mineły 24h od poprzedniego limitu";
                return View("Limits", currentLimit);
            }
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }

            Limit newLimit = new Limit { Date = DateTime.Now, Amount = limit, UserAccountId = user.Id, UserAccount = user };
            _context.Limits.Add(newLimit);
            _context.SaveChanges();
            currentLimit = newLimit;
            ViewData["Confirm"] = "Dodano nowy limit";

            return View("Limits", currentLimit);
        }




    }
}
