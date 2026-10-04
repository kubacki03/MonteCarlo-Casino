using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;

namespace MonteCarlo.NET.Controllers
{
    [Authorize]
    public class FootballBetController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IFootballBetService _footballBets;

        public FootballBetController(UserManager<UserAccount> userManager, IFootballBetService footballBets)
        {
            _userManager = userManager;
            _footballBets = footballBets;
        }

        public async Task<IActionResult> GetAvailableMatches()
        {
            var user = await _userManager.GetUserAsync(User);
            var overview = await _footballBets.GetOverviewAsync(user);

            ViewData["Saldo"] = user.Balance;
            ViewData["Level"] = user.Level;

            return View(new MatchesAndBetsViewModel
            {
                Matches = overview.Matches,
                Bets = overview.Bets
            });
        }

        [HttpPost]
        public async Task<IActionResult> SetBet(int matchId, long stake, int teamId)
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _footballBets.PlaceBetAsync(user, matchId, teamId, stake);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("GetAvailableMatches", "FootballBet");
            }

            TempData["Message"] = "Zakład został pomyślnie złożony!";
            return RedirectToAction("GetAvailableMatches");
        }
    }
}
