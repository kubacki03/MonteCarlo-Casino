using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Controllers
{
    public class LimitsController : Controller
    {
        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly KasynoContext _context;
        private readonly SignInManager<KontoUzytkownika> _signInManager;
        public LimitsController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, KasynoContext context, SignInManager<KontoUzytkownika> signInManager)
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
            var currentLimit = _context.Limit
            .Where(l => l.KontoUzytkownikaId == user.Id)
            .AsEnumerable() 
            .OrderBy(l => Math.Abs((l.Data - DateTime.Now).Ticks)) 
            .FirstOrDefault();

            if (user != null)
            {
                ViewData["Level"] = user.Level;
                ViewData["Saldo"] = user.Saldo;
            }
            return View(currentLimit);
        }


        [HttpPost]
        public async Task<IActionResult> SetLimit(long limit)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            if (user != null && await _userManager.IsLockedOutAsync(user))
            {

                await _signInManager.SignOutAsync();
                TempData["ErrorMessage"] = "Twoje konto zostało zablokowane na 15 minut.";
                return RedirectToAction("Login");
            }

            var currentLimit = _context.Limit
         .Where(l => l.KontoUzytkownikaId == user.Id)
         .AsEnumerable()
         .OrderBy(l => Math.Abs((l.Data - DateTime.Now).Ticks))
         .FirstOrDefault();


            if (currentLimit != null && DateTime.Now < currentLimit.Data.AddDays(1) )
            {
                ViewData["Error"] = "Nie mineły 24h od poprzedniego limitu";
                return View("Limits", currentLimit);
            }
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }

            Limit newLimit = new Limit { Data =DateTime.Now , Kwota=limit , KontoUzytkownikaId=user.Id, KontoUzytkownika=user};
            _context.Limit.Add(newLimit);
            _context.SaveChanges();
            currentLimit = newLimit;
            ViewData["Confirm"] = "Dodano nowy limit";

            return View("Limits",currentLimit);
        }


      

    }
}
