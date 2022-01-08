using Consul;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace IdentityService.Api.Extensions
{
    public static class ConsulExtensions
    {
        public static void ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(x => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));
        }

        public static void RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var features = app.ServerFeatures;

            if (features is not null)
            {
                // INFO : Get server IP address
                var addressses = features.Get<IServerAddressesFeature>();

                // if (addressses is not null)
                // {
                // var address = addressses.Addresses.First();

                // var uri = new Uri(address);

                var registration = new AgentServiceRegistration
                {
                    ID = "IdentityService",
                    Name = "IdentityService",
                    Address = "localhost",
                    Port = 5005,
                    Tags = new[] { "IdentityService", "" }
                };

                logger.LogInformation("Registering with Consul");

                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                consulClient.Agent.ServiceRegister(registration).Wait();

                lifetime.ApplicationStopping.Register(() =>
                {
                    logger.LogInformation("ServiceDeregister from consul");
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                });
            }

        }
    }
}