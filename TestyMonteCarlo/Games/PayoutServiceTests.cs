using Microsoft.AspNetCore.DataProtection;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace TestyMonteCarlo.Games
{
    public class PayoutServiceTests
    {
        private sealed class RecordingEmailService : IEmailService
        {
            public int? SentCode { get; private set; }
            public string? SentReportUrl { get; private set; }

            public void SendPayoutConfirmation(string email, int code, string reportUrl)
            {
                SentCode = code;
                SentReportUrl = reportUrl;
            }
        }

        private static PayoutService CreateService(TestDatabase database, out RecordingEmailService email)
        {
            email = new RecordingEmailService();
            return new PayoutService(null!, database.CreateContext(), email, new EphemeralDataProtectionProvider());
        }

        [Theory]
        [InlineData(0, "123")]
        [InlineData(-1, "123")]
        [InlineData(10, " ")]
        [InlineData(10, null)]
        [InlineData(1000, "123")]
        [InlineData(long.MaxValue, "123")]
        public void ValidateRequest_RejectsBadInput(long amount, string? account)
        {
            using var database = new TestDatabase();
            var service = CreateService(database, out _);

            Assert.NotNull(service.ValidateRequest(new UserAccount { Balance = 100 }, amount, account));
        }

        [Fact]
        public void ValidateRequest_AcceptsAffordableAmount()
        {
            using var database = new TestDatabase();
            var service = CreateService(database, out _);

            Assert.Null(service.ValidateRequest(new UserAccount { Balance = 100 }, 100, "PL123"));
        }

        [Fact]
        public void StartChallenge_EmailsTheCodeAndReportLink()
        {
            using var database = new TestDatabase();
            var service = CreateService(database, out var email);
            var user = new UserAccount { Id = "u1", Email = "a@b.c", Balance = 100 };

            var challenge = service.StartChallenge(user, 10, "PL123", token => "https://x/report?token=" + token);

            Assert.Equal(challenge.Code, email.SentCode);
            Assert.StartsWith("https://x/report?token=", email.SentReportUrl);
            Assert.InRange(challenge.Code, 1000, 9999);
        }

        [Fact]
        public async Task ConfirmAsync_CorrectCodeDebitsBalanceAndRecordsTransaction()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new PayoutService(null!, context, new RecordingEmailService(), new EphemeralDataProtectionProvider());
            var challenge = service.StartChallenge(user, 40, "PL123", t => t);

            var outcome = await service.ConfirmAsync(user, challenge, challenge.Code);

            Assert.Equal(PayoutConfirmation.Completed, outcome);
            Assert.Equal(60, TestData.BalanceOf(database, user.Id));
            using var verify = database.CreateContext();
            Assert.Equal("Wyplata", Assert.Single(verify.Transactions).Type);
        }

        [Fact]
        public async Task ConfirmAsync_WrongCodeLocksOutAfterMaxAttempts()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new PayoutService(null!, context, new RecordingEmailService(), new EphemeralDataProtectionProvider());
            var challenge = service.StartChallenge(user, 40, "PL123", t => t);
            var wrong = challenge.Code == 1000 ? 1001 : 1000;

            for (var i = 1; i < PayoutService.MaxCodeAttempts; i++)
            {
                Assert.Equal(PayoutConfirmation.WrongCode, await service.ConfirmAsync(user, challenge, wrong));
            }

            Assert.Equal(PayoutConfirmation.TooManyAttempts, await service.ConfirmAsync(user, challenge, wrong));
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task ConfirmAsync_ExpiredChallengeIsRejected()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new PayoutService(null!, context, new RecordingEmailService(), new EphemeralDataProtectionProvider());
            var challenge = service.StartChallenge(user, 40, "PL123", t => t);
            challenge.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(-1);

            Assert.Equal(PayoutConfirmation.Expired, await service.ConfirmAsync(user, challenge, challenge.Code));
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task ConfirmAsync_RejectsWhenBalanceDroppedSinceRequest()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new PayoutService(null!, context, new RecordingEmailService(), new EphemeralDataProtectionProvider());
            var challenge = service.StartChallenge(user, 40, "PL123", t => t);
            user.Balance = 10;

            Assert.Equal(PayoutConfirmation.InsufficientFunds, await service.ConfirmAsync(user, challenge, challenge.Code));
        }

        [Fact]
        public async Task ReportUnauthorizedAsync_InvalidTokenIsRejected()
        {
            using var database = new TestDatabase();
            var service = CreateService(database, out _);

            var result = await service.ReportUnauthorizedAsync("garbage");

            Assert.Equal(PayoutReportOutcome.InvalidToken, result.Outcome);
        }
    }
}
