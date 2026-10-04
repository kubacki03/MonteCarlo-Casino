namespace test.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using MonteCarlo.NET.Controllers;
    using MonteCarlo.NET.Data;
    using MonteCarlo.NET.Models;

    public class ZdrapkaKoniczykaController : Controller
    {

        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly KasynoContext _context;
        public ZdrapkaKoniczykaController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, KasynoContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }


        private static ZdrapkaKoniczynka _game;


        [HttpGet]
        [Route("game2")]
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

            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Zdrapka koniczynka"));


            if (user.Saldo < gra.MinStawka)
            {

                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, aby zagrać w tę grę. Musisz mieć co najmniej " + gra.MinStawka + " zł";
                return RedirectToAction("Zdrapki", "Home");
            }


            /*if (sumaKwot == null)
            {
                sumaKwot = 0;
            }*/


            if (limit != null)
            {
                Console.WriteLine("Wydanop " + sumaKwot);
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit";
                    return RedirectToAction("Zdrapki", "Home");
                }
            }

            long prize = 0;
            _game = new ZdrapkaKoniczynka();
            _game.InitGame();
            user.Saldo -= gra.MinStawka;
            if (_game.CheckForWin()) { 
            user.Saldo += _game.Wygrana;
                prize = _game.Wygrana;
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

            GraKonto graKonto = new GraKonto {IdGry=gra.IdGry, IlePostawiono=gra.MinStawka, IleWygrano=prize, KontoUzytkownikaId=user.Id};
            _context.GraKonto.Add(graKonto);
            _context.SaveChanges();

            return View("Index", _game);
        }

       


    }
}
