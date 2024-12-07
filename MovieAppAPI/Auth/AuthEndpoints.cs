using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MovieAppAPI.Auth.Model;

namespace MovieAppAPI.Auth
{
	public static class AuthEndpoints
	{
		public static void AddAuth(this WebApplication app)
		{
			app.MapPost("/api/auth/register", async (RegisterDto request, UserManager<User> userManager) =>
			{
				var user = await userManager.FindByEmailAsync(request.Email);
				if (user != null)
				{
					return Results.UnprocessableEntity("User with this email already exists");
				}

				var newUser = new User
				{
					UserName = request.Username,
					Email = request.Email
				};
				var createUser = await userManager.CreateAsync(newUser, request.Password);
				if (!createUser.Succeeded)
				{
					return Results.UnprocessableEntity(createUser.Errors);
				}

				await userManager.AddToRoleAsync(newUser, Roles.User.ToString());
				return Results.Created("/api/auth/login", new UserDto(newUser.Id, newUser.Email));
			});
			app.MapPost("/api/auth/login",
				async (UserManager<User> userManager, LoginDto request, JwtTokenGenerator createToken,
					HttpContext context) =>
				{
					if (context.Request.Cookies.TryGetValue("refreshToken", out var cookieRefreshToken))
					{
						var isTokenValid = createToken.ValidateRefreshToken(cookieRefreshToken, out var claims);
						if (isTokenValid)
						{
							var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
							var existingUser = await userManager.FindByIdAsync(userId);

							if (existingUser == null)
							{
								return Results.UnprocessableEntity();
							}

							var userRoles = await userManager.GetRolesAsync(existingUser);
							var newAccessToken = createToken.GenerateAccessToken(existingUser.Id, existingUser.UserName!, userRoles.ToList());
							return Results.Ok(new Success(newAccessToken));
						}
					}

					var user = await userManager.FindByNameAsync(request.Username);
					if (user == null)
					{
						return Results.UnprocessableEntity("User does not exist");
					}

					var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);

					if (!isPasswordValid)
					{
						return Results.UnprocessableEntity("Invalid password");
					}

					var roles = await userManager.GetRolesAsync(user);
					var accessToken = createToken.GenerateAccessToken(user.Id, user.UserName!, roles.ToList());
					var refreshToken = createToken.GenerateRefreshToken(user.Id);

					context.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
					{
						HttpOnly = true,
						SameSite = SameSiteMode.Lax,
						Expires = DateTime.UtcNow.AddDays(7)
					});

					return Results.Ok(new Success(accessToken));
				});
			app.MapPost("/api/auth/refresh", async (JwtTokenGenerator createToken, HttpContext context, UserManager<User> userManager) =>
			{
				if (context.Request.Cookies.TryGetValue("refreshToken", out var cookieRefreshToken))
				{
					var isTokenValid = createToken.ValidateRefreshToken(cookieRefreshToken, out var claims);
					if (isTokenValid)
					{
						var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
						var existingUser = await userManager.FindByIdAsync(userId);

						if (existingUser == null)
						{
							return Results.UnprocessableEntity();
						}

						var userRoles = await userManager.GetRolesAsync(existingUser);
						var newAccessToken = createToken.GenerateAccessToken(existingUser.Id, existingUser.UserName!, userRoles.ToList());
						return Results.Ok(new Success(newAccessToken));
					}
				}
				
				return Results.Unauthorized();
				
			});
			app.MapPost("api/auth/logout", (HttpContext context) =>
			{
				context.Response.Cookies.Delete("refreshToken");
				return Results.Ok();
			});
		}

		public record RegisterDto(string Username, string Email, string Password);

		public record Success(string AccessToken);

		public record UserDto(string UserId, string Email);

		public record LoginDto(string Username, string Password);

	}
}
