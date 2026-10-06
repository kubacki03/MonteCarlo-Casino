using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace MonteCarlo.NET.Controllers
{
    public class PayoutController : Controller
    {
        private const string ChallengeSessionKey = "payoutChallenge";

        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
        private readonly IPayoutService _payouts;

        public PayoutController(
            UserManager<UserAccount> userManager,
            SignInManager<UserAccount> signInManager,
            IPayoutService payouts)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _payouts = payouts;
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
        public async Task<IActionResult> Report(string token)
        {
            var result = await _payouts.ReportUnauthorizedAsync(token);
            return result.Outcome switch
            {
                PayoutReportOutcome.Reported => Ok(result.Message),
                PayoutReportOutcome.AccountNotFound => NotFound(result.Message),
                _ => BadRequest(result.Message)
            };
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayout(long amount, string accountNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Balance;

            var error = _payouts.ValidateRequest(user, amount, accountNumber);
            if (error != null)
            {
                TempData["ErrorMessage"] = error;
                return View("Payout");
            }

            var challenge = _payouts.StartChallenge(
                user,
                amount,
                accountNumber,
                token => Url.Action(nameof(Report), "Payout", new { token }, Request.Scheme)!);
            SaveChallenge(challenge);

            return View("Verification");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmedPayout(long code)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Balance;

            var challenge = LoadChallenge();
            if (challenge == null)
            {
                return RejectPayout("Kod wygasł, rozpocznij wypłatę od nowa");
            }

            switch (await _payouts.ConfirmAsync(user, challenge, code))
            {
                case PayoutConfirmation.Completed:
                    HttpContext.Session.Remove(ChallengeSessionKey);
                    ViewData["Saldo"] = user.Balance;
                    return View("Success");
                case PayoutConfirmation.WrongCode:
                    SaveChallenge(challenge);
                    TempData["ErrorMessage"] = "Podany zły kod";
                    return View("Verification");
                case PayoutConfirmation.TooManyAttempts:
                    return RejectPayout("Zbyt wiele błędnych prób, rozpocznij wypłatę od nowa");
                case PayoutConfirmation.InsufficientFunds:
                    return RejectPayout("Masz za malo brigmacoinsow");
                default:
                    return RejectPayout("Kod wygasł, rozpocznij wypłatę od nowa");
            }
        }

        private IActionResult RejectPayout(string message)
        {
            HttpContext.Session.Remove(ChallengeSessionKey);
            TempData["ErrorMessage"] = message;
            return View("Payout");
        }

        private void SaveChallenge(PayoutChallenge challenge)
        {
            HttpContext.Session.SetString(ChallengeSessionKey, JsonSerializer.Serialize(challenge));
        }

        private PayoutChallenge? LoadChallenge()
        {
            var json = HttpContext.Session.GetString(ChallengeSessionKey);
            return json == null ? null : JsonSerializer.Deserialize<PayoutChallenge>(json);
        }
    }
}
