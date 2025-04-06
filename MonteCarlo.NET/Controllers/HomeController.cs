using GSF.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace MonteCarlo.NET.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<KontoUzytkownika> _signInManager;
        private readonly KasynoContext _context;
        public HomeController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, SignInManager<KontoUzytkownika> signInManager, KasynoContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _context = context;
        }


        [HttpGet]
        [Route("/getName")]
        public async Task<String> getName()
        {
            var user = await _userManager.GetUserAsync(User);

            return user.Imie;
        }

        public async Task<IActionResult> IndexAsync()
        {
        

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }


            return View();
        }

        public async Task<IActionResult> BotSupport()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            return View("Chatbot");
        }
        public async Task<IActionResult> Privacy()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SlotsyAsync()
        {
            var u = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);
            if (user != null) { 
            ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            var graSlotsy = _context.Gra.FirstOrDefault(g => g.Nazwa == "Slotsy");

            return View(graSlotsy.MinStawka);
        }


        [HttpGet]
        [Route("liveChat")]
        [Authorize]
        public async Task<IActionResult> LiveChat()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            return View();
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ZwrocWynik([FromBody] JsonElement dane)
        {
            bool wygrana = dane.GetProperty("wygrana").GetBoolean();
            double stawka = dane.GetProperty("stawka").GetDouble();

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

            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Slotsy"));


            if (user.Saldo < gra.MinStawka || user.Saldo < stawka)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("SlotsyAsync");
            }

            if (gra.MinStawka > stawka)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {gra.MinStawka} BrigmaCoinsow";
                return RedirectToAction("SlotsyAsync");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + sumaKwot);
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("SlotsyAsync");
                }
            }
            user.Saldo -= stawka;

            double nagroda = 0;
            if (wygrana)
            {
                nagroda = 100 * stawka;
                user.Saldo += nagroda;
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

            GraKonto graKonto = new GraKonto()
            {
                KontoUzytkownikaId = user.Id,
                IdGry = gra.IdGry,
                IlePostawiono = stawka,
                IleWygrano = nagroda,
                Czas = DateTime.Now
            };
            try
            {
                _context.Users.Update(user);
                _context.GraKonto.Add(graKonto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu do bazy danych: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Wewnętrzny błąd: {ex.InnerException.Message}");
                }
            }

            return RedirectToAction("SlotsyAsync");
        }

        [Authorize]
        [HttpPost]
        [Route("/RC/ZwrocWynikRuletka")]
        public async Task<IActionResult> ZwrocWynikRuletka([FromBody] JsonElement dane)
        {
            bool wygrana = dane.GetProperty("isWinner").GetBoolean();
            double stawka = dane.GetProperty("allBets").GetDouble();
            double nagroda = dane.GetProperty("winnings").GetDouble();


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

            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Ruletka"));


            if (user.Saldo < gra.MinStawka || user.Saldo < stawka)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("RuletkaAsync");
            }

            if (gra.MinStawka > stawka)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {gra.MinStawka} BrigmaCoinsow";
                return RedirectToAction("RuletkaAsync");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + sumaKwot);
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("RuletkaAsync");
                }
            }
            user.Saldo -= stawka;
            user.Saldo += nagroda;

            var count = _context.GraKonto.Where(g => g.KontoUzytkownikaId == user.Id).ToList().Count;

            var level = _context.Levele
             .Where(l => l.MinimumPlayedGames <= count)
             .OrderByDescending(l => l.MinimumPlayedGames)
             .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;

            }

            GraKonto graKonto = new GraKonto()
            {
                KontoUzytkownikaId = user.Id,
                IdGry = gra.IdGry,
                IlePostawiono = stawka,
                IleWygrano = nagroda,
                Czas = DateTime.Now
            };
            try
            {
                _context.Users.Update(user);
                _context.GraKonto.Add(graKonto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu do bazy danych: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Wewnętrzny błąd: {ex.InnerException.Message}");
                }
            }

            return RedirectToAction("RuletkaAsync");
        }

        [Authorize]
        [HttpPost]
        [Route("/RC/ZwrocWynikKosci")]
        public async Task<IActionResult> ZwrocWynikKosci([FromBody] JsonElement dane)
        {
            bool wygrana = dane.GetProperty("isWinner").GetBoolean();
            double stawka = dane.GetProperty("bet").GetDouble();
            double nagroda = dane.GetProperty("ileWygrane").GetDouble();

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

            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Kosci"));


            if (user.Saldo < gra.MinStawka || user.Saldo < stawka)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("KosciAsync");
            }

            if (gra.MinStawka > stawka)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {gra.MinStawka} BrigmaCoinsow";
                return RedirectToAction("KosciAsync");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + sumaKwot);
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("KosciAsync");
                }
            }

            user.Saldo -= stawka;
            user.Saldo += nagroda;

            var count = _context.GraKonto.Where(g => g.KontoUzytkownikaId == user.Id).ToList().Count;

            var level = _context.Levele
             .Where(l => l.MinimumPlayedGames <= count)
             .OrderByDescending(l => l.MinimumPlayedGames)
             .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;

            }

            GraKonto graKonto = new GraKonto()
            {
                KontoUzytkownikaId = user.Id,
                IdGry = gra.IdGry,
                IlePostawiono = stawka,
                IleWygrano = nagroda,
                Czas = DateTime.Now
            };
            try
            {
                _context.Users.Update(user);
                _context.GraKonto.Add(graKonto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd zapisu do bazy danych: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Wewnętrzny błąd: {ex.InnerException.Message}");
                }
            }

            if (wygrana)
            {
                TempData["ErrorMessage"] = "Wygrane!";
            }
            else if (nagroda == stawka)
            {
                TempData["ErrorMessage"] = "Remis!";
            } else
            {
                TempData["ErrorMessage"] = "Przegrane!";
            }

            return RedirectToAction("KosciAsync");
        }

        [Authorize]
        [HttpPost]
        [Route("/RC/CheckBet")]
        public async Task<IActionResult> CheckBet([FromBody] JsonElement dane)
        {
            double stawka = dane.GetProperty("betData").GetDouble();

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

            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Kosci"));


            if (user.Saldo < gra.MinStawka || user.Saldo < stawka)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return Json(false);
            }

            if (gra.MinStawka > stawka)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {gra.MinStawka} BrigmaCoinsow";
                return Json(false);
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + sumaKwot);
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return Json(false);
                }
            }

            return Json(true);
        }
        

        [Authorize]
        public async Task<IActionResult> Kosci()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            return View();
        }


        [Authorize]
        public async Task<IActionResult> Ranking()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }


            var mostFrequentUserId = _context.GraKonto
     .GroupBy(g => g.KontoUzytkownikaId)
     .OrderByDescending(group => group.Count())
     .Select(group => group.Key)
     .FirstOrDefault();


            var count = _context.GraKonto.Where(g => g.KontoUzytkownikaId == user.Id).ToList().Count;

            var level = _context.Levele
     .Where(l => l.MinimumPlayedGames <= count)
     .OrderByDescending(l => l.MinimumPlayedGames)
     .FirstOrDefault();

            var nextLevel = _context.Levele.FirstOrDefault(g => g.NumberOfLevel == level.NumberOfLevel + 1);

            var diff = nextLevel.MinimumPlayedGames - count;
            var bestUser = _context.KontoUzytkownika.FirstOrDefault(g => g.Id.Equals(mostFrequentUserId));
            ViewData["ToNextLevel"] = diff;
            ViewData["BestPlayer"] = bestUser.UserName;

            var levelList = _context.Levele.ToList();

            return View(levelList);
        }

        [Authorize]
        public async Task<IActionResult> ZdrapkiAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }

            ViewData["Zdrapka Prosta"] =  _context.Gra.FirstOrDefault(g => (g.Nazwa == "Zdrapka Prosta")).MinStawka;
        
            ViewData["Zdrapka Koniczynka"] = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Zdrapka Koniczynka")).MinStawka;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> PaymentAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Ruletka()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Wallet()
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

            var transactionList = _context.Transakcja.Where(t => (t.KontoUzytkownikaId.Equals(user.Id))).ToList();    

            return View(transactionList);
        }


        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CustomError()
        {
            // Możesz dodać dodatkowe informacje o błędzie w ViewData lub ViewBag
            return View("CustomError");
        }

        public IActionResult Stream()
        {
            return View();
        }



        public async void UserLevels()
        {
            var user = await _userManager.GetUserAsync(User);
           

            var count= _context.GraKonto.Where(g => g.KontoUzytkownikaId == user.Id).ToList().Count;

            var level = _context.Levele
     .Where(l => l.MinimumPlayedGames <= count)
     .OrderByDescending(l => l.MinimumPlayedGames) 
     .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;
                _context.SaveChanges();
            }

            
        }

    }
}
