using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Controllers
{
    public class ReportController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly CasinoContext _context;
        private readonly SignInManager<UserAccount> _signInManager;
        public ReportController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, CasinoContext context, SignInManager<UserAccount> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult ReportForm()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> SubmitReport(ReportFormViewModel form)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                Report report = new Report()
                {
                    UserAccountId = user.Id,
                    Title = form.Title,
                    Content = form.Content,
                    Notes = form.Notes,
                    Date = DateTime.Now,
                    Status = "Nowe"
                };
                _context.Reports.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View("ReportForm", form);

        }
    }
}
