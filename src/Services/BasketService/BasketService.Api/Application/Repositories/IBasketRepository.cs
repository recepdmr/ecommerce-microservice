using BasketService.Api.Domain.Models;

namespace BasketService.Api.Application.Repositories
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetAsync(string customerId);
        IEnumerable<string> GetUsers();
        Task<CustomerBasket> UpdateAsync(CustomerBasket basket);
        Task<bool> DeleteAsync(string id);
    }
}