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
        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<KontoUzytkownika> _signInManager;
        private readonly KasynoContext _context;
        public UserStatsController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, SignInManager<KontoUzytkownika> signInManager, KasynoContext context)
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
                ViewData["Saldo"] = user.Saldo;
                ViewData["Level"] = user.Level;
            }

            var spentMoney = _context.GraKonto.Where(g => g.KontoUzytkownikaId.Equals(user.Id)).Sum(r => r.IlePostawiono);
            var wonMoney = _context.GraKonto.Where(g => g.KontoUzytkownikaId.Equals(user.Id)).Sum(r => r.IleWygrano);

            var favouriteGameId = _context.GraKonto
                .Where(g => g.KontoUzytkownikaId.Equals(user.Id))
                .GroupBy(g => g.IdGry)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            var favGame = _context.Gra.FirstOrDefault(g => g.IdGry.Equals(favouriteGameId))?.Nazwa ?? "Nie znaleziono gry";

            Dictionary<int, long> data = new Dictionary<int, long>();

            for (int i = 1; i <= 24; i++)
            {
                var howManyPlayed = _context.GraKonto
                    .Where(g => g.KontoUzytkownikaId.Equals(user.Id)
                        && g.Czas.Hour == i)
                    .Count();

                data.Add(i, howManyPlayed);
            }

            // Przekazywanie danych do widoku
            ViewBag.SpentMoney = spentMoney;
            ViewBag.WonMoney = wonMoney;
            ViewBag.FavGame = favGame;
            ViewBag.Data = JsonConvert.SerializeObject(data);


            return View();
    }


}
}
