namespace BasketService.Api.Domain.Models
{
    public class CustomerBasket
    {
        public CustomerBasket(string buyerId)
        {
            BuyerId = buyerId;
            Items = new List<BasketItem>();
        }

        public string BuyerId { get; set; }

        public List<BasketItem> Items { get; set; }
    }
}