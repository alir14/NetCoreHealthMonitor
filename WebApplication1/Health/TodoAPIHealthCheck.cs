using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace WebApplication1.Health
{
    public class TodoAPIHealthCheck : IHealthCheck
    {
        private readonly HealthCheck _setting;
        private readonly IHttpClientFactory _httpClientFactory;
        public TodoAPIHealthCheck(IOptions<HealthCheck> options, 
            IHttpClientFactory httpClientFactory)
        {
            _setting = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("externalAPI");

                var requestMessage = new HttpRequestMessage(HttpMethod.Head, 
                    _setting.URITodo);

                var response = await client.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy();
                else
                    return HealthCheckResult.Degraded();

            }
            catch (Exception ex)
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, 
                    ex.Message);
            }
        }
    }
}
