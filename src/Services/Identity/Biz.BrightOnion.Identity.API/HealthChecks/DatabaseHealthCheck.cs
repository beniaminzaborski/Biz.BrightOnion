using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Identity.API.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            if (IsDatabaseAvailable())
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Database is alive."));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("Database unavailable."));
        }

        private bool IsDatabaseAvailable() { return false; }
    }
}
