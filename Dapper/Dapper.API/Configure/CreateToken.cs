using Dapper.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dapper.API.Configure
{
    public class CreateToken : ICreateToken
    {
        private readonly IConfiguration _configuration;

        public CreateToken(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentException(nameof(configuration));
        }

        /// <summary>
        /// User specific token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CreateTokenMethod(string userId, TokenData? data = null) //int userId, UserVerifyData obj, string fbtk
        {
            return CreateTokenMethod(userId, null, data);
        }

        /// <summary>
        /// Device specific token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string CreateTokenMethod(string userId, int? deviceId, TokenData? data = null) //int userId, UserVerifyData obj, string fbtk
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretForKey"]));

            var credentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("userId", userId.ToString())
                //
            };

            if (deviceId != null)
            {
                claims.Add(new Claim("deviceId", deviceId.ToString()));
            }

            if (data != null)
            {
                claims.Add(new Claim(nameof(data.Email), data.Email));

                if (data.AdditionalProperties is not null && data.AdditionalProperties.Any())
                {
                    foreach (var prop in data.AdditionalProperties)
                    {
                        claims.Add(new Claim(prop.Key, prop.Value.ToString()));
                    }
                }
            }

            //設定token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: null,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials
                );

            // 將Token轉換為字串
            return new JwtSecurityTokenHandler().WriteToken(token);
            //var tokenHandler = new JwtSecurityTokenHandler();
            //return tokenHandler.WriteToken(token);
        }

        public JwtSecurityToken ReadToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            return jsonToken;
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenS = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (tokenS == null)
                return null;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                                       Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretForKey"]))
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }

        public async Task<(bool isValid, int userId)> ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var securityKey = new SymmetricSecurityKey(
              Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretForKey"]));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = securityKey
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var memberId = int.Parse(jwtToken.Claims.First(x => x.Type == "member_id").Value);

                return (true, memberId);
            }
            catch (Exception)
            {

                return (false, 0);
            }

        }
    }
}
