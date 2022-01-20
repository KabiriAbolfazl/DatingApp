using Microsoft.Extensions.Configuration;
using WebApi.Entities;
using WebApi.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace WebApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey key;
        public TokenService(IConfiguration configuration)
        {
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        }
        public string CreateToken(AppUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name,appUser.UserName)
            };

            var cerds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = cerds,
                Expires = DateTime.Now.AddDays(7)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
