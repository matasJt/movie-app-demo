using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MovieAppAPI.Auth.Model;

namespace MovieAppAPI.Auth
{
	public class JwtTokenGenerator(IConfiguration configuration)
	{
		public readonly string Issuer = configuration["Jwt:ValidIssuer"]!;
		public readonly string Audience = configuration["Jwt:ValidAudience"]!;
		public readonly SymmetricSecurityKey Key = new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

		public string GenerateAccessToken(string userId, string userName, List<string> roles)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new(JwtRegisteredClaimNames.Sub, userId),
				new(JwtRegisteredClaimNames.Name, userName),
			};

			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(10),
				Issuer = Issuer,
				Audience = Audience,
				SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
		public string GenerateRefreshToken(string userId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new(JwtRegisteredClaimNames.Sub, userId),
			};

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7),
				Issuer = Issuer,
				Audience = Audience,
				SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
		public bool ValidateRefreshToken(string refreshToken, out ClaimsPrincipal? claims)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = Issuer,
				ValidAudience = Audience,
				IssuerSigningKey = Key
			};
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				claims = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out _);
				return true;
			}
			catch
			{
				claims = null;
				return false;
			}
			
		}
	}
}
