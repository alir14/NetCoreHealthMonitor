using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace WebApplication1.Health
{
    public class MySqlHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;
        private readonly string _testQuery;

        public MySqlHealthCheck(string connectionString, string testQuery)
        {
            _connectionString = connectionString;
            _testQuery = testQuery;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = _testQuery;

                    await cmd.ExecuteNonQueryAsync(cancellationToken);

                    return HealthCheckResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, ex.Message);
            }
        }
    }
}
