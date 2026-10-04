namespace MonteCarlo.NET.Services.Games
{
    public sealed record PlayResult<T>(bool Succeeded, T? Outcome, string? ErrorMessage)
    {
        public static PlayResult<T> Success(T outcome) => new(true, outcome, null);

        public static PlayResult<T> Failure(string errorMessage) => new(false, default, errorMessage);
    }
}
