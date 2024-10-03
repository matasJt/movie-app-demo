using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MovieAppAPI.Data.Entities;
using Npgsql.PostgresTypes;

namespace MovieAppAPI.Data
{
	public class MoviesDbContext(IConfiguration configuration) : DbContext
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
