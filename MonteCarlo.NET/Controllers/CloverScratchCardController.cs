using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;

namespace MonteCarlo.NET.Controllers
{
    [Authorize]
    public class CloverScratchCardController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IScratchCardService _scratchCards;

        public CloverScratchCardController(UserManager<UserAccount> userManager, IScratchCardService scratchCards)
        {
            _userManager = userManager;
            _scratchCards = scratchCards;
        }

        [HttpGet]
        [Route("game2")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _scratchCards.PlayCloverAsync(user);

            ViewData["Saldo"] = user.Balance;
            ViewData["Level"] = user.Level;

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction("ScratchCards", "Home");
            }

            return View("Index", result.Outcome);
        }
    }
}
