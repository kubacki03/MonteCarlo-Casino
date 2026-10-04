
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MonteCarlo.NET.Controllers;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;
using Stripe.Checkout;

public class PaymentController : Controller
{

    private readonly UserManager<UserAccount> _userManager;
    private readonly ILogger<HomeController> _logger;
    private readonly CasinoContext _context;

    string apiKey = Environment.GetEnvironmentVariable("STRIPE_KEY");

    public PaymentController(ILogger<HomeController> logger, UserManager<UserAccount> userManager, CasinoContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }


    [HttpPost]
    public async Task<IActionResult> CreateCheckoutSession(long amount)
    {

        var client = new Stripe.StripeClient(apiKey);
        var user = await _userManager.GetUserAsync(User);
        ViewData["Saldo"] = user.Balance;
        var lineItems = new List<SessionLineItemOptions>
    {
        new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (amount)*100,
                Currency = "pln",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = "Doładowanie konta o " + amount+" brigmacoinsów"
                }
            },
            Quantity = 1
        }
    };

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card", "klarna", "blik" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme),
            CancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme)
        };

        var service = new SessionService(client);
        Session session = service.Create(options);
        var id = session.Id;

        TempData["SessionId"] = id;


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
        var client = new Stripe.StripeClient(apiKey);

        var service = new SessionService(client);
        var session = service.Get(sessionId);

        double charge = (double)session.AmountTotal / 100;

        var user = await _userManager.GetUserAsync(User);

        user.Balance += charge;


        Transaction transaction = new Transaction { Date = DateTime.Now, Amount = charge, Type = "Wplata", UserAccount = user, UserAccountId = user.Id };
        _context.Add(transaction);
        _context.SaveChanges();
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
