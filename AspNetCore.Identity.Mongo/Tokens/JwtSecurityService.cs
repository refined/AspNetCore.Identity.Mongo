using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCore.Identity.Mongo.Tokens
{
    public class JwtSecurityService
    {
        private readonly JwtSettings _settings;

        private SymmetricSecurityKey SecurityKey =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

        public int ExpiresInSeconds => (_settings.TokenExpirationMinutes ?? 24 * 60) * 60;

        public JwtSecurityService(IOptions<JwtSettings> settings) 
            : this(settings.Value)
        {
        }

        public JwtSecurityService(JwtSettings settings)
        {
            _settings = settings;
        }


        public TokenValidationParameters GetValidationParameters() =>
            new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _settings.Audience,

                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SecurityKey,

                ValidateLifetime = true,
            };

        public JwtSecurityToken GetToken(IEnumerable<Claim> claims) =>
            new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(ExpiresInSeconds),
                signingCredentials: new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256));

        public string GetTokenString(IEnumerable<Claim> claims) =>
            new JwtSecurityTokenHandler().WriteToken(GetToken(claims));
    }
}
