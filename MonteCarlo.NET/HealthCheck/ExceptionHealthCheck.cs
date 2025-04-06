using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MonteCarlo.NET.HealthCheck
{
    public class ExceptionHealthCheck : IHealthCheck
    {
        private readonly ExceptionTracker _exceptionTracker;

        public ExceptionHealthCheck(ExceptionTracker exceptionTracker)
        {
            _exceptionTracker = exceptionTracker;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var lastException = _exceptionTracker.GetLastException();
            if (lastException != null)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Wystąpił wyjątek w aplikacji: "+ lastException.Message+""+lastException.StackTrace,
                    lastException));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Brak problemów w aplikacji."));
        }
    }

}
