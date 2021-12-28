using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Api.Application.Services;
public class DefaultIdentityService : IIdentityService
{
    public Task<LoginResponseModel> LoginAsync(LoginRequestModel requestModel)
    {

        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier,requestModel.Username),
            new Claim(ClaimTypes.Name,"Recep Demir")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EcommerceProjectSecretKey_EcommerceProjectSecretKey"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(10);

        var token = new JwtSecurityToken(claims: claims, expires: expiry, signingCredentials: creds, notBefore: DateTime.Now);
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new LoginResponseModel
        {
            Username = requestModel.Username,
            UserToken = encodedJwt
        });
    }
}
