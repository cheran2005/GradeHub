using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GradifyApi.Service
{
    public class JwtTokenService 
    {
        //Configuration object for token information
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        //Generate a token based on PublicId
        public string GenerateToken(string PublicId, string email)
        {

            //Encode jwt key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            //Creating signature for JWT token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Creating claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, PublicId),
                new Claim(ClaimTypes.Email, email),//JWT token user identifier
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())//unique jwt token identifier
            };

            //Creating token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: creds
            );

            //Returning token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}