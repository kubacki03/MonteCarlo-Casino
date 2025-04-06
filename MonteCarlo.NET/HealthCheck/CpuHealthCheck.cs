using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace MonteCarlo.NET.HealthCheck
{
    public class CpuHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var cpuUsage = GetCpuUsage();
            if (cpuUsage < 80) 
            {
                return Task.FromResult(HealthCheckResult.Healthy($"CPU usage: {cpuUsage}%"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"High CPU usage: {cpuUsage}%"));
            }
        }

        private double GetCpuUsage()
        {
            using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
            {
                cpuCounter.NextValue(); 
                System.Threading.Thread.Sleep(500); 
                return cpuCounter.NextValue();
            }
        }
    }
}