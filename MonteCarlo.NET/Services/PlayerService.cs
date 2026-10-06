using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services
{
    public sealed record RankingInfo(IReadOnlyList<Level> Levels, long GamesToNextLevel, string? BestPlayer);

    public interface IPlayerService
    {
        Task<RankingInfo> GetRankingAsync(UserAccount user);

        Task<IReadOnlyList<Transaction>> GetTransactionsAsync(UserAccount user);
    }

    public class PlayerService : IPlayerService
    {
        private readonly CasinoContext _context;

        public PlayerService(CasinoContext context)
        {
            _context = context;
        }

        public async Task<RankingInfo> GetRankingAsync(UserAccount user)
        {
            var levels = await _context.Levels.OrderBy(l => l.NumberOfLevel).ToListAsync();
            var playedGames = await _context.GameAccounts.CountAsync(g => g.UserAccountId == user.Id);

            var current = levels
                .Where(l => l.MinimumPlayedGames <= playedGames)
                .OrderByDescending(l => l.MinimumPlayedGames)
                .FirstOrDefault();
            var next = current == null
                ? levels.FirstOrDefault()
                : levels.FirstOrDefault(l => l.NumberOfLevel == current.NumberOfLevel + 1);
            var gamesToNextLevel = next == null ? 0 : Math.Max(0, next.MinimumPlayedGames - playedGames);

            var mostActiveUserId = await _context.GameAccounts
                .GroupBy(g => g.UserAccountId)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefaultAsync();
            var bestPlayer = mostActiveUserId == null
                ? null
                : await _context.UserAccounts
                    .Where(u => u.Id == mostActiveUserId)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();

            return new RankingInfo(levels, gamesToNextLevel, bestPlayer);
        }

        public async Task<IReadOnlyList<Transaction>> GetTransactionsAsync(UserAccount user)
        {
            return await _context.Transactions.Where(t => t.UserAccountId == user.Id).ToListAsync();
        }
    }
}
