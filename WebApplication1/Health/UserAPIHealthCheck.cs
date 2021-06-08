using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Model;

namespace WebApplication1.Health
{
    public class UserAPIHealthCheck : IHealthCheck
    {

        private readonly HealthCheck _setting;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserAPIHealthCheck(IOptions<HealthCheck> options,
            IHttpClientFactory factory)
        {
            _httpClientFactory = factory;
            _setting = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("userAPI");

                var requestMessage = new HttpRequestMessage(HttpMethod.Head,
                    _setting.URIUser);

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
