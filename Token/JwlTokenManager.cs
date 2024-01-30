using Api.Token.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Token
{
    public class JwlTokenManager
    {
        private readonly IConfiguration _configuration;

        public JwlTokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Authenticate(LoginApplication application)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, application.User_name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddYears(999),
                    signingCredentials: creds
                );
            
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
