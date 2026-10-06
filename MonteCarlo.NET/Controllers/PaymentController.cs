using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace MonteCarlo.NET.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly IPaymentService _payments;

        public PaymentController(UserManager<UserAccount> userManager, IPaymentService payments)
        {
            _userManager = userManager;
            _payments = payments;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(long amount)
        {
            var session = await _payments.CreateCheckoutSessionAsync(
                amount,
                Url.Action("Success", "Payment", null, Request.Scheme)!,
                Url.Action("Cancel", "Payment", null, Request.Scheme)!);
            if (session == null)
            {
                return RedirectToAction("Cancel");
            }

            TempData["SessionId"] = session.Id;
            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Success()
        {
            var sessionId = TempData["SessionId"]?.ToString();
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Cancel");
            }

            var user = await _userManager.GetUserAsync(User);
            if (await _payments.CompletePaymentAsync(user, sessionId) == null)
            {
                return RedirectToAction("Cancel");
            }

            ViewData["Saldo"] = user.Balance;
            ViewData["Level"] = user.Level;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Saldo"] = user.Balance;
            ViewData["Level"] = user.Level;
            return View();
        }
    }
}
