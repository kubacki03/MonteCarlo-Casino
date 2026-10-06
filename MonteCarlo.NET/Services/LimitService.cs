using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services
{
    public sealed record SetLimitResult(bool Succeeded, Limit? Limit, string? ErrorMessage);

    public interface ILimitService
    {
        Task<Limit?> GetCurrentLimitAsync(UserAccount user);

        Task<SetLimitResult> SetLimitAsync(UserAccount user, long amount);
    }

    public class LimitService : ILimitService
    {
        public static readonly TimeSpan ChangeInterval = TimeSpan.FromDays(1);

        private readonly CasinoContext _context;

        public LimitService(CasinoContext context)
        {
            _context = context;
        }

        public Task<Limit?> GetCurrentLimitAsync(UserAccount user)
        {
            return _context.Limits
                .Where(l => l.UserAccountId == user.Id)
                .OrderByDescending(l => l.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<SetLimitResult> SetLimitAsync(UserAccount user, long amount)
        {
            var current = await GetCurrentLimitAsync(user);

            if (amount <= 0)
            {
                return new SetLimitResult(false, current, "Limit musi być większy od zera");
            }

            if (current != null && DateTime.Now < current.Date.Add(ChangeInterval))
            {
                return new SetLimitResult(false, current, "Nie mineły 24h od poprzedniego limitu");
            }

            var limit = new Limit { Date = DateTime.Now, Amount = amount, UserAccountId = user.Id };
            _context.Limits.Add(limit);
            await _context.SaveChangesAsync();
            return new SetLimitResult(true, limit, null);
        }
    }
}
