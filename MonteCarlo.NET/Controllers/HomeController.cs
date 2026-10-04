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
        private readonly UserManager<UserAccount> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly CasinoContext _context;
        public HomeController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager, CasinoContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _context = context;
        }


        [HttpGet]
        [Route("/getName")]
        public async Task<String> GetName()
        {
            var user = await _userManager.GetUserAsync(User);

            return user.FirstName;
        }

        public async Task<IActionResult> Index()
        {
        

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }


            return View();
        }

        public async Task<IActionResult> BotSupport()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }
            return View("Chatbot");
        }
        public async Task<IActionResult> Privacy()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Slots()
        {
            var u = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);
            if (user != null) { 
            ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }
            var slotsGame = _context.Games.FirstOrDefault(g => g.Name == "Slotsy");

            return View(slotsGame.MinStake);
        }


        [HttpGet]
        [Route("liveChat")]
        [Authorize]
        public async Task<IActionResult> LiveChat()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }
            return View();
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SubmitSlotsResult([FromBody] JsonElement payload)
        {
            bool isWinner = payload.GetProperty("wygrana").GetBoolean();
            double stake = payload.GetProperty("stawka").GetDouble();

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

            var game = _context.Games.FirstOrDefault(g => (g.Name == "Slotsy"));


            if (user.Balance < game.MinStake || user.Balance < stake)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("Slots");
            }

            if (game.MinStake > stake)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {game.MinStake} BrigmaCoinsow";
                return RedirectToAction("Slots");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + totalStaked);
                if (totalStaked + game.MinStake > limit.Amount)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("Slots");
                }
            }
            user.Balance -= stake;

            double prize = 0;
            if (isWinner)
            {
                prize = 100 * stake;
                user.Balance += prize;
            }
            var count = _context.GameAccounts.Where(g => g.UserAccountId == user.Id).ToList().Count;

            var level = _context.Levels
     .Where(l => l.MinimumPlayedGames <= count)
     .OrderByDescending(l => l.MinimumPlayedGames)
     .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;

            }

            GameAccount gameAccount = new GameAccount()
            {
                UserAccountId = user.Id,
                GameId = game.GameId,
                AmountStaked = stake,
                AmountWon = prize,
                PlayedAt = DateTime.Now
            };
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

            return RedirectToAction("Slots");
        }

        [Authorize]
        [HttpPost]
        [Route("/RC/SubmitRouletteResult")]
        public async Task<IActionResult> SubmitRouletteResult([FromBody] JsonElement payload)
        {
            bool isWinner = payload.GetProperty("isWinner").GetBoolean();
            double stake = payload.GetProperty("allBets").GetDouble();
            double prize = payload.GetProperty("winnings").GetDouble();


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

            var game = _context.Games.FirstOrDefault(g => (g.Name == "Ruletka"));


            if (user.Balance < game.MinStake || user.Balance < stake)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("Roulette");
            }

            if (game.MinStake > stake)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {game.MinStake} BrigmaCoinsow";
                return RedirectToAction("Roulette");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + totalStaked);
                if (totalStaked + game.MinStake > limit.Amount)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("Roulette");
                }
            }
            user.Balance -= stake;
            user.Balance += prize;

            var count = _context.GameAccounts.Where(g => g.UserAccountId == user.Id).ToList().Count;

            var level = _context.Levels
             .Where(l => l.MinimumPlayedGames <= count)
             .OrderByDescending(l => l.MinimumPlayedGames)
             .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;

            }

            GameAccount gameAccount = new GameAccount()
            {
                UserAccountId = user.Id,
                GameId = game.GameId,
                AmountStaked = stake,
                AmountWon = prize,
                PlayedAt = DateTime.Now
            };
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

            return RedirectToAction("Roulette");
        }

        [Authorize]
        [HttpPost]
        [Route("/RC/SubmitDiceResult")]
        public async Task<IActionResult> SubmitDiceResult([FromBody] JsonElement payload)
        {
            bool isWinner = payload.GetProperty("isWinner").GetBoolean();
            double stake = payload.GetProperty("bet").GetDouble();
            double prize = payload.GetProperty("ileWygrane").GetDouble();

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

            var game = _context.Games.FirstOrDefault(g => (g.Name == "Kosci"));


            if (user.Balance < game.MinStake || user.Balance < stake)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return RedirectToAction("Dice");
            }

            if (game.MinStake > stake)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {game.MinStake} BrigmaCoinsow";
                return RedirectToAction("Dice");
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + totalStaked);
                if (totalStaked + game.MinStake > limit.Amount)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return RedirectToAction("Dice");
                }
            }

            user.Balance -= stake;
            user.Balance += prize;

            var count = _context.GameAccounts.Where(g => g.UserAccountId == user.Id).ToList().Count;

            var level = _context.Levels
             .Where(l => l.MinimumPlayedGames <= count)
             .OrderByDescending(l => l.MinimumPlayedGames)
             .FirstOrDefault();


            if (user.Level != level.NumberOfLevel)
            {
                user.Level = level.NumberOfLevel;

            }

            GameAccount gameAccount = new GameAccount()
            {
                UserAccountId = user.Id,
                GameId = game.GameId,
                AmountStaked = stake,
                AmountWon = prize,
                PlayedAt = DateTime.Now
            };
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

            if (isWinner)
            {
                TempData["ErrorMessage"] = "Wygrane!";
            }
            else if (prize == stake)
            {
                TempData["ErrorMessage"] = "Remis!";
            } else
            {
                TempData["ErrorMessage"] = "Przegrane!";
            }

            return RedirectToAction("Dice");
        }

        [Authorize]
        [HttpPost]
        [Route("/RC/CheckBet")]
        public async Task<IActionResult> CheckBet([FromBody] JsonElement payload)
        {
            double stake = payload.GetProperty("betData").GetDouble();

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

            var game = _context.Games.FirstOrDefault(g => (g.Name == "Kosci"));


            if (user.Balance < game.MinStake || user.Balance < stake)
            {
                TempData["ErrorMessage"] = "Twoje saldo jest zbyt niskie, możesz grać ale nie będziesz w stanie nic wygrać.";
                return Json(false);
            }

            if (game.MinStake > stake)
            {
                TempData["ErrorMessage"] = $"Jeśli chcesz grać za brigmaCoinsy musisz grać za co najmniej {game.MinStake} BrigmaCoinsow";
                return Json(false);
            }

            if (limit != null)
            {
                Console.WriteLine("Wydano " + totalStaked);
                if (totalStaked + game.MinStake > limit.Amount)
                {
                    TempData["ErrorMessage"] = "Przekroczono limit, możesz grać ale nie będziesz w stanie nic wygrać.";
                    return Json(false);
                }
            }

            return Json(true);
        }
        

        [Authorize]
        public async Task<IActionResult> Dice()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
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
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }


            var mostFrequentUserId = _context.GameAccounts
     .GroupBy(g => g.UserAccountId)
     .OrderByDescending(group => group.Count())
     .Select(group => group.Key)
     .FirstOrDefault();


            var count = _context.GameAccounts.Where(g => g.UserAccountId == user.Id).ToList().Count;

            var level = _context.Levels
     .Where(l => l.MinimumPlayedGames <= count)
     .OrderByDescending(l => l.MinimumPlayedGames)
     .FirstOrDefault();

            var nextLevel = _context.Levels.FirstOrDefault(g => g.NumberOfLevel == level.NumberOfLevel + 1);

            var diff = nextLevel.MinimumPlayedGames - count;
            var bestUser = _context.UserAccounts.FirstOrDefault(g => g.Id.Equals(mostFrequentUserId));
            ViewData["ToNextLevel"] = diff;
            ViewData["BestPlayer"] = bestUser.UserName;

            var levelList = _context.Levels.ToList();

            return View(levelList);
        }

        [Authorize]
        public async Task<IActionResult> ScratchCards()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }

            ViewData["Zdrapka Prosta"] =  _context.Games.FirstOrDefault(g => (g.Name == "Zdrapka Prosta")).MinStake;
        
            ViewData["Zdrapka Koniczynka"] = _context.Games.FirstOrDefault(g => (g.Name == "Zdrapka Koniczynka")).MinStake;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Payment()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Roulette()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
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
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }

            if (user != null && await _userManager.IsLockedOutAsync(user))
            {
              
                await _signInManager.SignOutAsync(); 
                TempData["ErrorMessage"] = "Twoje konto zostało zablokowane na 15 minut.";
                return RedirectToAction("Login");
            }

            var transactionList = _context.Transactions.Where(t => (t.UserAccountId.Equals(user.Id))).ToList();    

            return View(transactionList);
        }


        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CustomError()
        {
            return View("CustomError");
        }

        public IActionResult Stream()
        {
            return View();
        }



        public async void UserLevels()
        {
            var user = await _userManager.GetUserAsync(User);
           

            var count= _context.GameAccounts.Where(g => g.UserAccountId == user.Id).ToList().Count;

            var level = _context.Levels
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
