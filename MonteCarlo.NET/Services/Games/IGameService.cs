using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services.Games
{
    public interface IGameService
    {
        Task<string?> GetPlayRejectionAsync(UserAccount user, string gameName);

        Task<string?> GetStakeRejectionAsync(UserAccount user, string gameName, double stake);

        Task<double?> GetMinStakeAsync(string gameName);

        Task<PlayResult<TOutcome>> PlayAsync<TOutcome>(
            UserAccount user,
            string gameName,
            double stake,
            Func<Game, (double Prize, TOutcome Outcome)> play);

        Task<PlayResult<TOutcome>> PlayAtMinimumStakeAsync<TOutcome>(
            UserAccount user,
            string gameName,
            Func<Game, (double Prize, TOutcome Outcome)> play);

        Task<T> InTransactionAsync<T>(Func<Task<T>> action);

        Task InTransactionAsync(Func<Task> action);
    }
}
