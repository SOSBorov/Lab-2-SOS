using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Api.Configuration;
using TodoApp.Models;

namespace TodoApp.Api.Services;

public class JwtTokenService
{
	private readonly JwtSettings _jwtSettings;

	public JwtTokenService(IOptions<JwtSettings> jwtOptions)
	{
		_jwtSettings = jwtOptions.Value;
	}

	public string CreateToken(User user)
	{
		List<Claim> claims =
		[
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, user.Email),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Role, user.Role)
		];

		SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtSettings.Key));
		SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

		JwtSecurityToken token = new(
			issuer: _jwtSettings.Issuer,
			audience: _jwtSettings.Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
