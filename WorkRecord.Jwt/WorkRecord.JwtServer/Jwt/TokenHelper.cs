using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkRecord.Model.Entity;
using WorkRecord.Model.Jwt;

namespace WorkRecord.JwtServer.Jwt
{
    public class TokenHelper : ITokenHelper
    {
        private IOptions<JWTConfig> _options;

        /// <summary>
        /// 构造函数依赖注入
        /// </summary>
        /// <param name="options"></param>
        public TokenHelper(IOptions<JWTConfig> options)
        {
            _options = options;
        }
        public TokenInfo CreateToken(User user)
        {
            Claim[] claims = new Claim[] { new Claim(ClaimTypes.Name, user.Account), new Claim(ClaimTypes.Name, user.Name) };

            return CreateToken(claims);
        }

        private TokenInfo CreateToken(Claim[] claims)
        {
            var now = DateTime.Now;
            var expires = now.Add(TimeSpan.FromMinutes(_options.Value.AccessTokenExpiresMinutes));
            var token = new JwtSecurityToken(
                issuer: _options.Value.Issuer,
                audience: _options.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.IssuerSigningKey)), SecurityAlgorithms.HmacSha256));
            return new TokenInfo { TokenContent = new JwtSecurityTokenHandler().WriteToken(token), TokenExpiresTime = expires };
        }
    }
}
