using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services
{
    public enum PayoutConfirmation
    {
        Completed,
        Expired,
        WrongCode,
        TooManyAttempts,
        InsufficientFunds
    }

    public enum PayoutReportOutcome
    {
        Reported,
        InvalidToken,
        AccountNotFound
    }

    public sealed class PayoutChallenge
    {
        public int Code { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public int Attempts { get; set; }
        public long Amount { get; set; }
        public string AccountNumber { get; set; } = "";
    }

    public sealed record PayoutReportResult(PayoutReportOutcome Outcome, string Message);

    public interface IPayoutService
    {
        string? ValidateRequest(UserAccount user, long amount, string? accountNumber);

        PayoutChallenge StartChallenge(UserAccount user, long amount, string accountNumber, Func<string, string> reportUrl);

        Task<PayoutConfirmation> ConfirmAsync(UserAccount user, PayoutChallenge challenge, long code);

        Task<PayoutReportResult> ReportUnauthorizedAsync(string token);
    }

    public class PayoutService : IPayoutService
    {
        public const int MaxCodeAttempts = 5;
        public static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan AccountLockDuration = TimeSpan.FromMinutes(15);

        private const string ReportTokenPurpose = "MonteCarlo.PayoutReport";
        private static readonly TimeSpan ReportLinkLifetime = TimeSpan.FromHours(24);

        private readonly UserManager<UserAccount> _userManager;
        private readonly CasinoContext _context;
        private readonly IEmailService _emailService;
        private readonly ITimeLimitedDataProtector _reportTokens;

        public PayoutService(
            UserManager<UserAccount> userManager,
            CasinoContext context,
            IEmailService emailService,
            IDataProtectionProvider dataProtection)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _reportTokens = dataProtection.CreateProtector(ReportTokenPurpose).ToTimeLimitedDataProtector();
        }

        public string? ValidateRequest(UserAccount user, long amount, string? accountNumber)
        {
            if (amount <= 0)
            {
                return "Kwota wypłaty musi być większa od zera";
            }
            if (amount > int.MaxValue)
            {
                return "Kwota wypłaty jest zbyt duża";
            }
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return "Podaj numer konta";
            }
            if ((user.Balance ?? 0) < amount)
            {
                return "Masz za malo brigmacoinsow";
            }

            return null;
        }

        public PayoutChallenge StartChallenge(UserAccount user, long amount, string accountNumber, Func<string, string> reportUrl)
        {
            var challenge = new PayoutChallenge
            {
                Code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1000, 10000),
                ExpiresAtUtc = DateTime.UtcNow.Add(CodeLifetime),
                Amount = amount,
                AccountNumber = accountNumber
            };

            var token = _reportTokens.Protect(user.Id, ReportLinkLifetime);
            _emailService.SendPayoutConfirmation(user.Email!, challenge.Code, reportUrl(token));
            return challenge;
        }

        public async Task<PayoutConfirmation> ConfirmAsync(UserAccount user, PayoutChallenge challenge, long code)
        {
            if (DateTime.UtcNow > challenge.ExpiresAtUtc)
            {
                return PayoutConfirmation.Expired;
            }

            challenge.Attempts++;
            if (code != challenge.Code)
            {
                return challenge.Attempts >= MaxCodeAttempts
                    ? PayoutConfirmation.TooManyAttempts
                    : PayoutConfirmation.WrongCode;
            }

            if ((user.Balance ?? 0) < challenge.Amount)
            {
                return PayoutConfirmation.InsufficientFunds;
            }

            _context.Transactions.Add(new Transaction
            {
                Date = DateTime.Now,
                Amount = challenge.Amount,
                UserAccountId = user.Id,
                Type = "Wyplata"
            });
            user.Balance -= challenge.Amount;
            await _context.SaveChangesAsync();
            return PayoutConfirmation.Completed;
        }

        public async Task<PayoutReportResult> ReportUnauthorizedAsync(string token)
        {
            string userId;
            try
            {
                userId = _reportTokens.Unprotect(token);
            }
            catch (Exception)
            {
                return new PayoutReportResult(PayoutReportOutcome.InvalidToken, "Link jest nieprawidłowy lub wygasł.");
            }

            var account = await _userManager.FindByIdAsync(userId);
            if (account == null)
            {
                return new PayoutReportResult(PayoutReportOutcome.AccountNotFound, "Nie znaleziono konta.");
            }

            _context.Reports.Add(new Report
            {
                Date = DateTime.Now,
                UserAccountId = account.Id,
                Status = "Przeslano",
                Content = "Zgloszono nieautoryzowaną próbę wypłaty z konta",
                Title = "Nieautoryzowana wyplata"
            });

            var lockoutEnd = DateTimeOffset.UtcNow.Add(AccountLockDuration);
            account.LockoutEnd = lockoutEnd;
            await _context.SaveChangesAsync();

            return new PayoutReportResult(
                PayoutReportOutcome.Reported,
                $"Zgłoszenie zostało wysłane, a konto zostało zablokowane do {lockoutEnd.ToLocalTime():g}.");
        }
    }
}
