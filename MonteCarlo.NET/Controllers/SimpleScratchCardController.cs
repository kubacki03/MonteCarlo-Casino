using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Controllers;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace test.Controllers
{
    public class ZdrapkaProstaController : Controller
    {
        private static readonly double[] Prizes = { 1, 5, 10, 0, 0, 0, 0, 0 };
        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly KasynoContext _context;
        public ZdrapkaProstaController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, KasynoContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }



        [HttpGet]
        [Route("game")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Saldo;
            ViewData["Level"] = user.Level;


            var sumaKwot = _context.GraKonto
            .Where(g => g.KontoUzytkownikaId == user.Id && g.Czas.Date == DateTime.Now.Date)
            .Sum(g => g.IlePostawiono);

            var limit = _context.Limit
      .Where(l => l.KontoUzytkownikaId == user.Id)
      .AsEnumerable()
      .OrderBy(l => Math.Abs((l.Data - DateTime.Now).Ticks))
      .FirstOrDefault();


            if (sumaKwot == null)
            {
                sumaKwot = 0;
            }


            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Zdrapka Prosta"));

            if (user.Saldo < gra.MinStawka)
            {

                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, aby zagrać w tę grę. Musisz mieć co najmniej " + gra.MinStawka + " zł";
                return RedirectToAction("Zdrapki", "Home");
            }


            if (sumaKwot == null)
            {
                sumaKwot = 0;
            }


            if (limit != null)
            {
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit";
                    return RedirectToAction("Zdrapki", "Home");
                }
            }
          


            return View();
        }


        [HttpGet]

        [HttpGet]
        [Route("generate")]
        public async Task<IActionResult> GeneratePrize()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Saldo;
            ViewData["Level"] = user.Level;

            var sumaKwot = _context.GraKonto
            .Where(g => g.KontoUzytkownikaId == user.Id && g.Czas.Date == DateTime.Now.Date)
            .Sum(g => g.IlePostawiono);

            var limit = _context.Limit
       .Where(l => l.KontoUzytkownikaId == user.Id)
       .AsEnumerable()
       .OrderBy(l => Math.Abs((l.Data - DateTime.Now).Ticks))
       .FirstOrDefault();

            if (sumaKwot == null)
            {
                sumaKwot = 0;
            }

            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Zdrapka Prosta"));


            if (user.Saldo < gra.MinStawka)
            {

                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, aby zagrać w tę grę. Musisz mieć co najmniej "+gra.MinStawka+" zł";
                return RedirectToAction("Zdrapki", "Home");
            }


            if (sumaKwot == null)
            {
                sumaKwot = 0;
            }


            if (limit != null)
            {
                Console.WriteLine("Wydanop " + sumaKwot);
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit";
                    return RedirectToAction("Zdrapki", "Home");
                }
            }
            var count = _context.GraKonto.Where(g => g.KontoUzytkownikaId == user.Id).ToList().Count;

            var level = _context.Levele
     .Where(l => l.MinimumPlayedGames <= count)
     .OrderByDescending(l => l.MinimumPlayedGames)
     .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;
                
            }

            user.Saldo -= gra.MinStawka;
            var random = new Random();
            var prize = Prizes[random.Next(Prizes.Length)];
            if (prize != 0)
            {
                prize = Prizes[random.Next(Prizes.Length)];
            }

            user.Saldo += prize;


            var game = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Zdrapka Prosta"));
            GraKonto graKonto = new GraKonto { IdGry = game.IdGry, IlePostawiono = gra.MinStawka, IleWygrano = prize, KontoUzytkownikaId = user.Id };
            _context.GraKonto.Add(graKonto);
            _context.SaveChanges();


            return Json(new { prize });
        }



    }

}