using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;

namespace MonteCarlo.NET.Controllers
{
    [Authorize]
    public class RouletteController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IRouletteService _roulette;

        public RouletteController(UserManager<UserAccount> userManager, IRouletteService roulette)
        {
            _userManager = userManager;
            _roulette = roulette;
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("RController/Spin")]
        public async Task<IActionResult> Spin([FromBody] RouletteSpinRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            var bets = request.Bets.Select(b => new RouletteBet(b.Position, b.Amount)).ToList();

            var result = await _roulette.SpinAsync(user, bets);
            if (!result.Succeeded)
            {
                return Json(new { succeeded = false, error = result.ErrorMessage });
            }

            return Json(new
            {
                succeeded = true,
                finalNumber = result.Outcome!.Number,
                stake = result.Outcome.Stake,
                winnings = result.Outcome.Winnings
            });
        }

        public sealed record RouletteSpinRequest(List<BetDto> Bets);

        public sealed record BetDto(int Position, int Amount);
    }
}
