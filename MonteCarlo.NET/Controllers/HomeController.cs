using GSF.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Services;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;
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
        private readonly ISlotService _slots;
        private readonly IDiceService _dice;
        private readonly IGameService _games;
        private readonly IPlayerService _players;
        public HomeController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager, ISlotService slots, IDiceService dice, IGameService games, IPlayerService players)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _slots = slots;
            _dice = dice;
            _games = games;
            _players = players;
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

            return View(await _games.GetMinStakeAsync(GameNames.Slots));
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SpinSlots([FromBody] SlotSpinRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _slots.SpinAsync(user, request.Stake);
            if (!result.Succeeded)
            {
                return Json(new { succeeded = false, error = result.ErrorMessage });
            }

            return Json(new
            {
                succeeded = true,
                symbols = result.Outcome!.Symbols,
                won = result.Outcome.Won,
                prize = result.Outcome.Prize
            });
        }

        public sealed record SlotSpinRequest(double Stake);

        [Authorize]
        [HttpPost]
        [Route("/RC/SubmitDiceResult")]
        public async Task<IActionResult> SubmitDiceResult([FromBody] JsonElement payload)
        {
            double stake = payload.GetProperty("bet").GetDouble();
            double prize = payload.GetProperty("ileWygrane").GetDouble();

            var user = await _userManager.GetUserAsync(User);
            var result = await _dice.SettleAsync(user, stake, prize);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return Json(new { succeeded = false, error = result.ErrorMessage });
            }

            TempData["ErrorMessage"] = result.Outcome!.Won ? "Wygrane!" : result.Outcome.Draw ? "Remis!" : "Przegrane!";
            return Json(new { succeeded = true });
        }

        [Authorize]
        [HttpPost]
        [Route("/RC/CheckBet")]
        public async Task<IActionResult> CheckBet([FromBody] JsonElement payload)
        {
            double stake = payload.GetProperty("betData").GetDouble();

            var user = await _userManager.GetUserAsync(User);
            var rejection = await _dice.CheckBetAsync(user, stake);
            if (rejection != null)
            {
                TempData["ErrorMessage"] = rejection;
                return Json(false);
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

            var ranking = await _players.GetRankingAsync(user);
            ViewData["ToNextLevel"] = ranking.GamesToNextLevel;
            ViewData["BestPlayer"] = ranking.BestPlayer;

            return View(ranking.Levels);
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

            ViewData["Zdrapka Prosta"] = await _games.GetMinStakeAsync(GameNames.SimpleScratchCard);
            ViewData["Zdrapka Koniczynka"] = await _games.GetMinStakeAsync(GameNames.CloverScratchCard);
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

            return View(await _players.GetTransactionsAsync(user));
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

    }
}
