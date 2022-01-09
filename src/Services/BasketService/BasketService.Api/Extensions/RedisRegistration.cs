using StackExchange.Redis;

namespace BasketService.Api.Extensions
{
    public static class RedisRegistration
    {
        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConf = ConfigurationOptions.Parse(configuration["RedisSettings:ConnectionString"], true);

            redisConf.ResolveDns = true;

            services.AddSingleton(sp => ConnectionMultiplexer.Connect(redisConf));
        }
    }
}