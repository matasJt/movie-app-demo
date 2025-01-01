using Microsoft.EntityFrameworkCore;
using MovieAppAPI.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieAppAPI.Auth.Model;

namespace MovieAppAPI.Data
{
	public class MoviesDbContext(IConfiguration configuration) : IdentityDbContext<User>
	{
		public DbSet<Tag> Tags { get; set; }
		public DbSet<Movie> Movies { get; set; }
		public DbSet<Review> Reviews { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));
		}
	}
}
