using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Biz.BrightOnion.Identity.API.Configuration;
using Biz.BrightOnion.Identity.API.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NGuard;

namespace Biz.BrightOnion.Identity.API.Services
{
  public class JwtAuthenticationService : IAuthenticationService
  {
    private readonly AppSettings appSettings;

    public JwtAuthenticationService(IOptions<AppSettings> appSettings)
    {
      this.appSettings = appSettings.Value;
    }

    public string CreateToken(User user)
    {
      Guard.Requires(user, nameof(user)).IsNotNull();

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(appSettings.Secret);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
          {
            new Claim(ClaimTypes.Name, user.Id.ToString())
          }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}
