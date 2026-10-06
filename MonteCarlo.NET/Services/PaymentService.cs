using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;
using Stripe;
using Stripe.Checkout;

namespace MonteCarlo.NET.Services
{
    public sealed record CheckoutSession(string Id, string Url);

    public interface IPaymentService
    {
        Task<CheckoutSession?> CreateCheckoutSessionAsync(long amount, string successUrl, string cancelUrl);

        Task<double?> CompletePaymentAsync(UserAccount user, string sessionId);
    }

    public class PaymentService : IPaymentService
    {
        private readonly CasinoContext _context;
        private readonly IStripeClient _client;

        public PaymentService(CasinoContext context, IStripeClient client)
        {
            _context = context;
            _client = client;
        }

        public async Task<CheckoutSession?> CreateCheckoutSessionAsync(long amount, string successUrl, string cancelUrl)
        {
            if (amount <= 0)
            {
                return null;
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card", "klarna", "blik" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = amount * 100,
                            Currency = "pln",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Doładowanie konta o {amount} brigmacoinsów"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var session = await new SessionService(_client).CreateAsync(options);
            return new CheckoutSession(session.Id, session.Url);
        }

        public async Task<double?> CompletePaymentAsync(UserAccount user, string sessionId)
        {
            var session = await new SessionService(_client).GetAsync(sessionId);
            if (session.PaymentStatus != "paid")
            {
                return null;
            }

            var charge = (session.AmountTotal ?? 0) / 100.0;
            user.Balance = (user.Balance ?? 0) + charge;
            _context.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Amount = charge,
                Type = "Wplata",
                UserAccountId = user.Id
            });
            await _context.SaveChangesAsync();
            return charge;
        }
    }
}
