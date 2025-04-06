using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Controllers
{
  
    public class FootballBetController : Controller
    {
        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<KontoUzytkownika> _signInManager;
        private readonly KasynoContext _context;
        public FootballBetController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, SignInManager<KontoUzytkownika> signInManager, KasynoContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _context = context;
        }




        public async Task<IActionResult> getAvailableMatches()
        {
           


            var user = await _userManager.GetUserAsync(User);
            var myBets = _context.Zaklady
                .Where(z => z.IdGracza == user.Id)
                .ToList();
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            foreach (var bet in myBets)
            {
                if (bet.czyPrzyznanoNagrode == false)
                {
                    var match = _context.Mecz.FirstOrDefault(m => m.MeczId == bet.IdMeczu);
                    if (match.data < DateOnly.FromDateTime(DateTime.Now))
                    {
                        Random random = new Random();
                        match.AwayTeamGoals = random.Next(5);
                        match.HomeTeamGoals = random.Next(5);

                        if (match.AwayTeamId == bet.IdZwyciezcy && match.AwayTeamGoals > match.HomeTeamGoals)
                        {
                            bet.czyWygral = true;
                            user.Saldo += match.AwayTeamOdds * bet.PostawionaKwota;
                            bet.czyPrzyznanoNagrode = true;
                        }
                        else if (match.HomeTeamId == bet.IdZwyciezcy && match.HomeTeamGoals > match.AwayTeamGoals)
                        {
                            bet.czyWygral = true;
                            user.Saldo += match.HomeTeamOdds * bet.PostawionaKwota;
                            bet.czyPrzyznanoNagrode = true;

                        }
                        else
                        {
                            bet.czyWygral = false;
                            bet.czyPrzyznanoNagrode = true;
                        }

                    }
                }
            }

        

            var matches = _context.Mecz.ToList();
          
            var viewModel = new MatchesAndBetsViewModel
            {
                Matches = matches,
                Bets = myBets
            };
            _context.SaveChanges();
            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> setBet(int MeczId, long Bet, int teamId)
        {

            Console.WriteLine("Mecz id " + MeczId + "  bet " + Bet + " teamid " + teamId);
            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Saldo;
            ViewData["Level"] = user.Level;
            var spentMoney = _context.GraKonto
            .Where(g => g.KontoUzytkownikaId == user.Id && g.Czas.Date == DateTime.Now.Date)
            .Sum(g => g.IlePostawiono);

            var limit = _context.Limit
        .Where(l => l.KontoUzytkownikaId == user.Id)
        .AsEnumerable()
        .OrderBy(l => Math.Abs((l.Data - DateTime.Now).Ticks))
        .FirstOrDefault();

            if (spentMoney == null)
            {
                spentMoney = 0;
            }

            var game = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Obstawianie"));


            if (user.Saldo < game.MinStawka)
            {

                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, aby zagrać w tę grę. Musisz mieć co najmniej " + game.MinStawka + " zł";
                return RedirectToAction("getAvailableMatches", "FootballBet");
            }


          


            if (limit != null)
            {
                Console.WriteLine("Wydanop " + spentMoney);
                if (spentMoney + game.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit";
                    return RedirectToAction("getAvailableMatches", "FootballBet");
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
            Zaklad newBet = new Zaklad { IdGracza = user.Id, IdMeczu = MeczId, IdZwyciezcy = teamId, PostawionaKwota = Bet, czyPrzyznanoNagrode =false };
            user.Saldo -= Bet;
            GraKonto graKonto = new GraKonto { Czas = DateTime.Now, IdGry = game.IdGry, IlePostawiono = Bet, IleWygrano = 0, KontoUzytkownikaId = user.Id };
            _context.GraKonto.Add(graKonto);
            _context.Zaklady.Add(newBet);
            _context.SaveChanges();
            TempData["Message"] = "Zakład został pomyślnie złożony!";
            return RedirectToAction("getAvailableMatches");
        }



    }
}
