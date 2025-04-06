using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace MonteCarlo.NET.Controllers
{
    public class PayoutController : Controller
    {

        private readonly UserManager<KontoUzytkownika> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly KasynoContext _context;
        private readonly SignInManager<KontoUzytkownika> _signInManager;
        public PayoutController(ILogger<HomeController> logger, UserManager<KontoUzytkownika> userManager, KasynoContext context, SignInManager<KontoUzytkownika> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }
        [Authorize]
        public async Task<IActionResult> ShowPayout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
            }
            if (user != null && await _userManager.IsLockedOutAsync(user))
            {
              
                await _signInManager.SignOutAsync(); 
                TempData["ErrorMessage"] = "Twoje konto zostało zablokowane na 15 minut.";
                return RedirectToAction("Login"); 
            }

            return View("Payout");
        }


        [HttpGet]
        [Route("api/payout/report")]
        public async Task<IActionResult> Report(string email)
        {
            var account = _context.KontoUzytkownika.FirstOrDefault(x => x.Email == email);

            if (account != null)
            {
                
                Zgloszenie report = new Zgloszenie
                {
                    Data = DateTime.Now,
                    KontoUzytkownikaId = account.Id,
                    Notatki = "java > c#",
                    Status = "Przeslano",
                    Tresc = "Zgloszono nieautoryzowaną próbę wypłaty z konta",
                    Tytul = "Nieautoryzowana wyplata",
                    KontoUzytkownika = account
                };
                _context.Zgloszenie.Add(report);
                _context.SaveChanges();

               
                var lockoutEndDate = DateTime.Now.AddMinutes(15);
                account.LockoutEnd = new DateTimeOffset(lockoutEndDate);

               
                _context.KontoUzytkownika.Update(account);
                await _context.SaveChangesAsync();

               
                return Ok($"Zgłoszenie zostało wysłane, a konto zostało zablokowane do {lockoutEndDate} .");
            }

            return NotFound("Nie znaleziono konta o podanym adresie e-mail.");
        }



        [Authorize]
        public async Task<IActionResult> CreatePayout(long kwota, string numer)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
            }
            if (user.Saldo < kwota)
            {
                TempData["ErrorMessage"] = "Masz za malo brigmacoinsow";

                return View("Payout");
            }
            Random random = new Random();
            int code = random.Next(1000, 10000);
            HttpContext.Session.SetInt32("kod", code);
            HttpContext.Session.SetInt32("kwota", (int)kwota);
            HttpContext.Session.SetString("numer",numer);
            EmailService emailService = new EmailService();
            emailService.SendEmail(user.Email, code, kwota);

            return View("Weryfikacja");

        }

        [Authorize]
        public async Task<IActionResult> ConfirmedPayout(long kod)
        {
            Console.WriteLine("TWoj kod " + kod);
            long ammount = (long)HttpContext.Session.GetInt32("kwota");
            string number = HttpContext.Session.GetString("numer");

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Saldo;
            }
            if(kod != HttpContext.Session.GetInt32("kod"))
            {

                TempData["ErrorMessage"] = "Podany zły kod";

                return View("Weryfikacja");
            }
            if (user.Saldo < ammount)
            {
                TempData["ErrorMessage"] = "Masz za malo brigmacoinsow";

                return View("Payout");
            }
            else
            {
                Transakcja transakcja = new Transakcja { Data = DateTime.Now, KontoUzytkownika = user, Kwota = ammount, KontoUzytkownikaId = user.Id, Typ="Wyplata" };
                _context.Add(transakcja);
                user.Saldo -= ammount;
                _context.SaveChanges();
            }
            HttpContext.Session.Remove("kod");
            ViewData["Saldo"] = user.Saldo;
            return View("Succes");
        }
    }
}
