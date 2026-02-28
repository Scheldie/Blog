using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Blog.Entities;
using Blog.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Services
{
    public class JwtProvider(IOptions<JwtOptions> options)
    {
        private readonly JwtOptions _options = options.Value;

        public string GenerateToken(User user)
        {
            Claim[] claims = [new("userId", user.Id.ToString())];
            var signinCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signinCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            
            return tokenValue;
        }
    }
    public class JwtOptions
    {
        public const string JwtConfig = "JwtConfig";
        public string SecretKey { get; set; } = "";

        public int ExpiresHours { get; set; }


    }
}
