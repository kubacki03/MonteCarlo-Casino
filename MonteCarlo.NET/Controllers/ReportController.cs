using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Controllers
{
    public class ZgloszenieController : Controller
    {
        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly KasynoContext _context;
        private readonly SignInManager<KontoUzytkownika> _signInManager;
        public ZgloszenieController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, KasynoContext context, SignInManager<KontoUzytkownika> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult FormularzZgloszenie()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> WyslijZgloszenie(FormularzZgloszenie formularzZgloszenie)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                Zgloszenie zgloszenie = new Zgloszenie()
                {
                    KontoUzytkownikaId = user.Id,
                    Tytul = formularzZgloszenie.Tytul,
                    Tresc = formularzZgloszenie.Tresc,
                    Notatki = formularzZgloszenie.Notatki,
                    Data = DateTime.Now,
                    Status = "Nowe"
                };
                _context.Zgloszenie.Add(zgloszenie);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View("FormularzZgloszenie", formularzZgloszenie);

        }
    }
}
