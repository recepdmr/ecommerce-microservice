using System.Text.Json;
using BasketService.Api.Application.Repositories;
using BasketService.Api.Domain.Models;
using StackExchange.Redis;

namespace BasketService.Api.Infrastructure.Repositories
{
    public class RedisBasketRepository : IBasketRepository
    {

        public RedisBasketRepository(ILogger<RedisBasketRepository> logger, ConnectionMultiplexer redis)
        {
            Logger = logger;
            Redis = redis;
            Database = redis.GetDatabase();
        }

        private IDatabase Database { get; set; }
        public ConnectionMultiplexer Redis { get; set; }
        public ILogger<RedisBasketRepository> Logger { get; }

        public async Task<bool> DeleteAsync(string id)
        {
            return await Database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetAsync(string customerId)
        {
            var data = await Database.StringGetAsync(customerId);
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<CustomerBasket>(data);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();

            var data = server.Keys();

            return data?.Select(x => x.ToString());
        }

        private IServer GetServer()
        {
            var endpoint = Redis.GetEndPoints();

            return Redis.GetServer(endpoint.First());
        }

        public async Task<CustomerBasket> UpdateAsync(CustomerBasket basket)
        {
            var created = await Database.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket));

            if (!created)
            {
                Logger.LogInformation("Problem occur persinsting the item.");

                return null;
            }

            Logger.LogInformation("Basket item persisted successfully.");

            return await GetAsync(basket.BuyerId);
        }
    }
}