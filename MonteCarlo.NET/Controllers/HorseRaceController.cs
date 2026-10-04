using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;


namespace MonteCarlo.NET.Controllers
{
    public class HorseRaceController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly CasinoContext _context;
        public HorseRaceController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, CasinoContext context)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._context = context;
        }
        public async Task<ActionResult> Index()
        {
            var horses = PrepareHorses();

            var model = new MonteCarlo.NET.Models.BetViewModel
            {
                AvailableHorses = horses
            };

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(MonteCarlo.NET.Models.BetViewModel model)
        {
            var horses = PrepareHorses();
            model.AvailableHorses = horses;

            if (string.IsNullOrEmpty(model.SelectedHorse) || model.Amount <= 0)
            {
                ModelState.AddModelError("", "Wybierz konia i wprowadź poprawną kwotę.");
                return View(model);
            }

            var race = new Race(horses);
            var winner = race.RunRace();
            model.IsRaceFinished = true;
            model.Winner = winner.Name;


            var horsesWithTimes = race.Horses.Zip(race.Times, (k, c) => new HorseResult { HorseName = k.Name, Time = c, Color = k.Color })
                                 .OrderBy(hr => hr.HorseName)
                                 .ToList();
            model.HorseResults = horsesWithTimes;




            if (model.SelectedHorse == winner.Name)
            {
                model.IsWinner = true;
                model.WinAmount = model.Amount * 3;
            }
            else
            {
                model.IsWinner = false;
                model.WinAmount = 0;
            }


            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Balance;
            ViewData["Level"] = user.Level;
            var totalStaked = _context.GameAccounts
                .Where(g => g.UserAccountId == user.Id && g.PlayedAt.Date == DateTime.Now.Date)
                .Sum(g => g.AmountStaked);

            var limit = _context.Limits
                .Where(l => l.UserAccountId == user.Id)
                .AsEnumerable()
                .OrderBy(l => Math.Abs((l.Date - DateTime.Now).Ticks))
                .FirstOrDefault();

            if (totalStaked == null)
            {
                totalStaked = 0;
            }

            var game = _context.Games.FirstOrDefault(g => (g.Name == "Wyscigi Konne"));


            if (user.Balance < game.MinStake || user.Balance < (double)model.Amount)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("Index");
            }

            if (game.MinStake > (double)model.Amount)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {game.MinStake} BrigmaCoinsow";
                return RedirectToAction("Index");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + totalStaked);
                if (totalStaked + game.MinStake > limit.Amount)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("Index");
                }
            }
            user.Balance -= (double)model.Amount;



            GameAccount gameAccount = new GameAccount()
            {
                UserAccountId = user.Id,
                GameId = game.GameId,
                AmountStaked = (double)model.Amount,
                AmountWon = (double)model.WinAmount,
                PlayedAt = DateTime.Now
            };
            var count = _context.GameAccounts.Where(g => g.UserAccountId == user.Id).ToList().Count;

            var level = _context.Levels
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
                _context.GameAccounts.Add(gameAccount);
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

            return View("Result", model);
        }

        private List<Horse> PrepareHorses()
        {
            return new List<Horse>
            {
                new Horse { Name = "Błyskawica", Age=4, Weight=500, Size=150, Color="Brąz", Speed=20, Stamina=1.0f},
                new Horse { Name = "Czempion", Age=5, Weight=520, Size=155, Color="Czarny", Speed=22, Stamina=1.04f},
                new Horse { Name = "Srebrzysta", Age=3, Weight=480, Size=148, Color="Srebrny", Speed=19, Stamina=1.1f},
                new Horse { Name = "Zefir", Age=6, Weight=550, Size=160, Color="Kasztan", Speed=21, Stamina=1.03f},
                new Horse { Name = "Burza", Age=5, Weight=500, Size=152, Color="Siwy", Speed=20.5f, Stamina=1.05f},
                new Horse { Name = "Piorun", Age=4, Weight=510, Size=150, Color="Kary", Speed=23, Stamina=1.02f},
                new Horse { Name = "Wicher", Age=5, Weight=490, Size=149, Color="Gniady", Speed=18, Stamina=1.15f},
                new Horse { Name = "Mistrz", Age=4, Weight=505, Size=151, Color="Dereszowaty", Speed=21.5f, Stamina=1.0f}
            };
        }
    }
}
