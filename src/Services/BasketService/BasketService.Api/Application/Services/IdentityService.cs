using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace BasketService.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            HttpContext = httpContextAccessor.HttpContext ?? throw new ArgumentException(nameof(httpContextAccessor.HttpContext));
        }

        public HttpContext HttpContext { get; }

        public int GetUserId()
        {
            throw new NotImplementedException();
        }

        public string GetUserName()
        {
            return HttpContext.User.FindFirst(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value ?? string.Empty;
        }
    }
}