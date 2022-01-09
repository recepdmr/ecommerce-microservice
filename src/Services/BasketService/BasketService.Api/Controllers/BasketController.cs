using BasketService.Api.Application.Repositories;
using BasketService.Api.Application.Services;
using BasketService.Api.Domain.Models;
using BasketService.Api.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        public BasketController(IBasketRepository basketRepository,
        IEventBus eventBus,
        IIdentityService identityService,
        ILogger<BasketController> logger)
        {
            BasketRepository = basketRepository;
            EventBus = eventBus;
            IdentityService = identityService;
            Logger = logger;
        }

        public IBasketRepository BasketRepository { get; }
        public IEventBus EventBus { get; }
        public IIdentityService IdentityService { get; }
        public ILogger<BasketController> Logger { get; }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Basket Service is up and running");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> GetAsync(string id)
        {
            var basket = await BasketRepository.GetAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        [Route("update")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateAsync([FromBody] CustomerBasket basket)
        {
            return Ok(await BasketRepository.UpdateAsync(basket));
        }

        [HttpPost]
        [Route("additem")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddItemAsync([FromBody] BasketItem basketItem)
        {
            var userId = IdentityService.GetUserName().ToString();

            var basket = await BasketRepository.GetAsync(userId);

            if (basket == null)
            {
                basket = new CustomerBasket(userId);
            }

            basket.Items.Add(basketItem);

            await BasketRepository.UpdateAsync(basket);

            return Ok();
        }

        [HttpPost]
        [Route("checkout")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
        {
            var userId = basketCheckout.Buyer;

            var basket = await BasketRepository.GetAsync(userId);

            if (basket is null)
            {
                return BadRequest();
            }

            var userName = IdentityService.GetUserName();


            var @event = new OrderCreatedIntegrationEvent(userId, userName, int.MaxValue, basketCheckout.City,
            basketCheckout.Street,
            basketCheckout.State,
            basketCheckout.Country,
            basketCheckout.ZipCode,
            basketCheckout.CardNumber,
            basketCheckout.CardHolderName,
            basketCheckout.CardExpiration,
            basketCheckout.CardSecurityNumber,
            basketCheckout.CardTypeId,
            basketCheckout.Buyer,
            basket
            );

            try
            {
                await EventBus.PublishAsync(@event);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex, "Publishing integration event : {IntegrationEventId} from {Service}", @event.Id, "BasketService.App");

                throw;
            }

            return Accepted();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync()
        {
            var userName = IdentityService.GetUserName();
            await BasketRepository.DeleteAsync(userName);

            return NoContent();
        }
    }
}