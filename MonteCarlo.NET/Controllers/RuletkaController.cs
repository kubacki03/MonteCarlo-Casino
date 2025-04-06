using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Controllers
{
    public class RuletkaController : Controller
    {
        private readonly ILogger<RuletkaController> _logger;

        public RuletkaController(ILogger<RuletkaController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Route("RController/PlaceBet")]
        public IActionResult PlaceBet([FromBody] BetRequest bet)
        {
            try
            {
                if (bet.Money <= 0 || bet.Position < 0)
                {
                    return BadRequest();
                }

                RuletkaFunkcje.bet_numbers(bet.Money, bet.Position);

                return StatusCode(204);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bet.");
                return StatusCode(500);
            }
        }



        [HttpGet]
        [Route("RController/GetBets")]
        public IActionResult GetBets()
        {
            return Json(RuletkaFunkcje.bets);
        }

        [HttpPost]
        [Route("RController/GetResult")]
        public IActionResult GetResult()
        {
            float winnings = RuletkaFunkcje.grac();
            float allBets = RuletkaFunkcje.gracDwa();
            
            bool winner = false;
            if (winnings > 0)
                winner = true;

            return Json(new
            {
                win = winner,
                bets = allBets,
                coins = winnings,
                finalNumber = RuletkaFunkcje.wynik
            });
        }
    }

    public class BetRequest
    {
        public float Money { get; set; }
        public int Position { get; set; }
    }
}
