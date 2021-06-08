using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApplication1.Model;
using System;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<HealthCheck>(Configuration.GetSection("HealthCheck"));

            services.AddHttpClient("externalAPI", client =>
            {
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
            });

            services.AddHttpClient("userAPI", client =>
            {
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
            });

            var setting = Configuration.Get<AppSetting>();

            services.AddControllers();

            services.AddHealthChecks()
                .AddTypeActivatedCheck<MySqlHealthCheck>(
                "db1HealthCheck",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "sql" },
                args: new object[] { setting.HealthCheck.SQLConnectionStringMaster, setting.HealthCheck.TestQuery })
                .AddTypeActivatedCheck<MySqlHealthCheck>(
                "db2HealthCheck",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "sql" },
                args: new object[] { setting.HealthCheck.SQLConnectionStringHealthCheck, setting.HealthCheck.TestQuery })
                .AddCheck<TodoAPIHealthCheck>("todoAPI", tags: new[] { "todo" })
                .AddCheck<UserAPIHealthCheck>("userAPI", tags: new[] { "user" });

            services.AddHealthChecksUI(setupSettings: setup => {
                setup.AddHealthCheckEndpoint("allEndpoint", "https://localhost:5001/allHealthChecks");
                setup.AddHealthCheckEndpoint("sqlEndpoint", "https://localhost:5001/dbHealthCheck");
                setup.AddHealthCheckEndpoint("apiEndpoint", "https://localhost:5001/APIHealthCheck");

            }).AddInMemoryStorage();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/allHealthChecks", new HealthCheckOptions() { 
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecks("/dbHealthCheck", new HealthCheckOptions() { 
                    Predicate = (check) => check.Tags.Contains("sql"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

                });


                endpoints.MapHealthChecks("/APIHealthCheck", new HealthCheckOptions()
                {
                    Predicate = (check) => (check.Tags.Contains("todo") || 
                        check.Tags.Contains("user")),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });

            app.UseHealthChecksUI(config => config.UIPath = "/healthUI");
        }
    }
}
