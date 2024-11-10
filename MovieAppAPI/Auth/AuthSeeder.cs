using Microsoft.AspNetCore.Identity;
using MovieAppAPI.Auth.Model;

namespace MovieAppAPI.Auth
{
	public class AuthSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
	{
		public async Task SeedAsync()
		{
			await AddRoles();
			await AddAdmin();
		}

		public async Task AddRoles()
		{
			foreach (var role in Enum.GetValues(typeof(Roles)))
			{
				var roleExists = await roleManager.RoleExistsAsync(role.ToString()!);
				if (!roleExists)
				{
					await roleManager.CreateAsync(new IdentityRole(role.ToString()!));
				}
			}
		}

		public async Task AddAdmin()
		{
			var admin = new User
			{
				UserName = "Admin",
				Email = "system@host.eth",
			};

			var adminExist = await userManager.FindByEmailAsync(admin.Email);
			if (adminExist == null)
			{
				await userManager.CreateAsync(admin, configuration["AdminPassword:Admin"]!);
				await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
				await userManager.AddToRoleAsync(admin, Roles.User.ToString());
			}
		}
	}
}
