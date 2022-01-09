namespace BasketService.Api.Application.Services
{
    public interface IIdentityService
    {
        int GetUserId();

        string GetUserName();
    }
}