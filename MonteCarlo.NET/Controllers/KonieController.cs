using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;


namespace MonteCarlo.NET.Controllers
{
    public class KonieController : Controller
    {
        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly KasynoContext _context;
        public KonieController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, KasynoContext context)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._context = context;
        }
        public async Task<ActionResult> KonieMenu()
        {
            // Przygotowanie listy 8 koni
            var konie = PrzygotujKonie();

            var model = new MonteCarlo.NET.Models.BetViewModel
            {
                DostepneKonie = konie
            };

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> KonieMenu(MonteCarlo.NET.Models.BetViewModel model)
        {
            var konie = PrzygotujKonie();
            model.DostepneKonie = konie;

            // Sprawdzenie poprawności danych
            if (string.IsNullOrEmpty(model.WybranyKon) || model.Kwota <= 0)
            {
                ModelState.AddModelError("", "Wybierz konia i wprowadź poprawną kwotę.");
                return View(model);
            }

            // Uruchomienie wyścigu
            var wyscig = new Wyscig(konie);
            var zwyciezca = wyscig.RozegrajWyscig();
            model.WyscigRozstrzygniety = true;
            model.Zwyciezca = zwyciezca.imie;


            var konieZCzasem = wyscig.Lista_Koni.Zip(wyscig.Czas, (k, c) => new HorseResult { KonImie = k.imie, Czas = c, Kolor = k.kolor })
                                 .OrderBy(hr => hr.KonImie)
                                 .ToList();
            model.WynikiKoni = konieZCzasem;




            // Sprawdź, czy użytkownik wygrał
            if (model.WybranyKon == zwyciezca.imie)
            {
                model.Wygrana = true;
                // Załóżmy, że wygrana to np. 3x stawka
                model.WygranaKwota = model.Kwota * 3;
            }
            else
            {
                model.Wygrana = false;
                model.WygranaKwota = 0;
            }

            //baza

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

            var gra = _context.Gra.FirstOrDefault(g => (g.Nazwa == "Wyscigi Konne"));


            if (user.Saldo < gra.MinStawka || user.Saldo < (double)model.Kwota)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("KonieMenu");
            }

            if (gra.MinStawka > (double)model.Kwota)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {gra.MinStawka} BrigmaCoinsow";
                return RedirectToAction("KonieMenu");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + sumaKwot);
                if (sumaKwot + gra.MinStawka > limit.Kwota)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("KonieMenu");
                }
            }
            user.Saldo -= (double)model.Kwota;



            GraKonto graKonto = new GraKonto()
            {
                KontoUzytkownikaId = user.Id,
                IdGry = gra.IdGry,
                IlePostawiono = (double)model.Kwota,
                IleWygrano = (double)model.WygranaKwota,
                Czas = DateTime.Now
            };
            var count = _context.GraKonto.Where(g => g.KontoUzytkownikaId == user.Id).ToList().Count;

            var level = _context.Levele
     .Where(l => l.MinimumPlayedGames <= count)
     .OrderByDescending(l => l.MinimumPlayedGames)
     .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;

            }
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

            return View("ResultKonie", model);
        }

        private List<Konie> PrzygotujKonie()
        {
            // Na potrzeby przykładu tworzymy 8 koni z losowymi parametrami
            return new List<Konie>
            {
                new Konie { imie = "Błyskawica", wiek=4, waga=500, rozmiar=150, kolor="Brąz", predkosc=20, wytrzymalosc=1.0f},
                new Konie { imie = "Czempion", wiek=5, waga=520, rozmiar=155, kolor="Czarny", predkosc=22, wytrzymalosc=1.04f},
                new Konie { imie = "Srebrzysta", wiek=3, waga=480, rozmiar=148, kolor="Srebrny", predkosc=19, wytrzymalosc=1.1f},
                new Konie { imie = "Zefir", wiek=6, waga=550, rozmiar=160, kolor="Kasztan", predkosc=21, wytrzymalosc=1.03f},
                new Konie { imie = "Burza", wiek=5, waga=500, rozmiar=152, kolor="Siwy", predkosc=20.5f, wytrzymalosc=1.05f},
                new Konie { imie = "Piorun", wiek=4, waga=510, rozmiar=150, kolor="Kary", predkosc=23, wytrzymalosc=1.02f},
                new Konie { imie = "Wicher", wiek=5, waga=490, rozmiar=149, kolor="Gniady", predkosc=18, wytrzymalosc=1.15f},
                new Konie { imie = "Mistrz", wiek=4, waga=505, rozmiar=151, kolor="Dereszowaty", predkosc=21.5f, wytrzymalosc=1.0f}
            };
        }
    }
}
