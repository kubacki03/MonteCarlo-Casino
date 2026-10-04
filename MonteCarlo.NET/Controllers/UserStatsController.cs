using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;
using Newtonsoft.Json;

namespace MonteCarlo.NET.Controllers
{
    public class UserStatsController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly CasinoContext _context;
        public UserStatsController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager, CasinoContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _context = context;
        }





[Authorize]
    public async Task<IActionResult> UserStats( )
    {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
                ViewData["Level"] = user.Level;
            }

            var spentMoney = _context.GameAccounts.Where(g => g.UserAccountId.Equals(user.Id)).Sum(r => r.AmountStaked);
            var wonMoney = _context.GameAccounts.Where(g => g.UserAccountId.Equals(user.Id)).Sum(r => r.AmountWon);

            var favouriteGameId = _context.GameAccounts
                .Where(g => g.UserAccountId.Equals(user.Id))
                .GroupBy(g => g.GameId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            var favGame = _context.Games.FirstOrDefault(g => g.GameId.Equals(favouriteGameId))?.Name ?? "Nie znaleziono gry";

            Dictionary<int, long> data = new Dictionary<int, long>();

            for (int i = 1; i <= 24; i++)
            {
                var howManyPlayed = _context.GameAccounts
                    .Where(g => g.UserAccountId.Equals(user.Id)
                        && g.PlayedAt.Hour == i)
                    .Count();

                data.Add(i, howManyPlayed);
            }

            ViewBag.SpentMoney = spentMoney;
            ViewBag.WonMoney = wonMoney;
            ViewBag.FavGame = favGame;
            ViewBag.Data = JsonConvert.SerializeObject(data);


            return View();
    }


}
}
