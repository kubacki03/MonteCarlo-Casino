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

        private readonly UserManager<UserAccount> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly CasinoContext _context;
        private readonly SignInManager<UserAccount> _signInManager;
        public PayoutController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, CasinoContext context, SignInManager<UserAccount> signInManager)
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
                ViewData["Saldo"] = user.Balance;
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
            var account = _context.UserAccounts.FirstOrDefault(x => x.Email == email);

            if (account != null)
            {
                
                Report report = new Report
                {
                    Date = DateTime.Now,
                    UserAccountId = account.Id,
                    Notes = "java > c#",
                    Status = "Przeslano",
                    Content = "Zgloszono nieautoryzowaną próbę wypłaty z konta",
                    Title = "Nieautoryzowana wyplata",
                    UserAccount = account
                };
                _context.Reports.Add(report);
                _context.SaveChanges();

               
                var lockoutEndDate = DateTime.Now.AddMinutes(15);
                account.LockoutEnd = new DateTimeOffset(lockoutEndDate);

               
                _context.UserAccounts.Update(account);
                await _context.SaveChangesAsync();

               
                return Ok($"Zgłoszenie zostało wysłane, a konto zostało zablokowane do {lockoutEndDate} .");
            }

            return NotFound("Nie znaleziono konta o podanym adresie e-mail.");
        }



        [Authorize]
        public async Task<IActionResult> CreatePayout(long amount, string accountNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
            }
            if (user.Balance < amount)
            {
                TempData["ErrorMessage"] = "Masz za malo brigmacoinsow";

                return View("Payout");
            }
            Random random = new Random();
            int code = random.Next(1000, 10000);
            HttpContext.Session.SetInt32("kod", code);
            HttpContext.Session.SetInt32("kwota", (int)amount);
            HttpContext.Session.SetString("numer",accountNumber);
            EmailService emailService = new EmailService();
            emailService.SendEmail(user.Email, code, amount);

            return View("Verification");

        }

        [Authorize]
        public async Task<IActionResult> ConfirmedPayout(long code)
        {
            Console.WriteLine("TWoj kod " + code);
            long amount = (long)HttpContext.Session.GetInt32("kwota");
            string number = HttpContext.Session.GetString("numer");

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewData["Saldo"] = user.Balance;
            }
            if(code != HttpContext.Session.GetInt32("kod"))
            {

                TempData["ErrorMessage"] = "Podany zły kod";

                return View("Verification");
            }
            if (user.Balance < amount)
            {
                TempData["ErrorMessage"] = "Masz za malo brigmacoinsow";

                return View("Payout");
            }
            else
            {
                Transaction transaction = new Transaction { Date = DateTime.Now, UserAccount = user, Amount = amount, UserAccountId = user.Id, Type="Wyplata" };
                _context.Add(transaction);
                user.Balance -= amount;
                _context.SaveChanges();
            }
            HttpContext.Session.Remove("kod");
            ViewData["Saldo"] = user.Balance;
            return View("Success");
        }
    }
}
