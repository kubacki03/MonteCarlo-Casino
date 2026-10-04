using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;

namespace MonteCarlo.NET.Controllers
{
    [Authorize]
    public class SimpleScratchCardController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IGameService _games;
        private readonly IScratchCardService _scratchCards;

        public SimpleScratchCardController(
            UserManager<UserAccount> userManager,
            IGameService games,
            IScratchCardService scratchCards)
        {
            _userManager = userManager;
            _games = games;
            _scratchCards = scratchCards;
        }

        [HttpGet]
        [Route("game")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Balance;
            ViewData["Level"] = user.Level;

            var rejection = await _games.GetPlayRejectionAsync(user, GameNames.SimpleScratchCard);
            if (rejection != null)
            {
                TempData["ErrorMessage"] = rejection;
                return RedirectToAction("ScratchCards", "Home");
            }

            return View();
        }

        [HttpGet]
        [Route("generate")]
        public async Task<IActionResult> GeneratePrize()
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _scratchCards.PlaySimpleAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("ScratchCards", "Home");
            }

            return Json(new { prize = result.Outcome });
        }
    }
}
