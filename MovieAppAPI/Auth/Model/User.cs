using Microsoft.AspNetCore.Identity;

namespace MovieAppAPI.Auth.Model
{
	public class User : IdentityUser
	{
		public bool forceLogin { get; set; }
	}
}
