using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieAppAPI.Data.Dtos;
using MovieAppAPI.Data.Entities;
using MovieAppAPI.Data;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MovieAppAPI.Endpoints
{
	public class MovieEndpoints
	{
		public static void MapMovieEndpoints(RouteGroupBuilder moviesGroup)
		{
			moviesGroup.MapGet("/tags/{tagId:int}/movies", async (MoviesDbContext dbContext, int tagId) =>
			{
				return (await dbContext.Tags.Where(x => x.Id == tagId).SelectMany(x => x.Movies).ToListAsync()).Select(x => x.ToDto());

			});
			moviesGroup.MapGet("/movies/{movieId:int}", async (MoviesDbContext dbContext, int movieId) =>
			{
				var movie = await dbContext.Movies.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == movieId);
				if (movie == null)
				{
					return Results.NotFound();
				}

				return Results.Ok(movie.ToDto());
			});
			moviesGroup.MapPost("/movies", async (MoviesDbContext dbContext, UpdateCreateMovieDto dto, HttpContext httpContext) =>
			{
				var movie = new Movie
				{
					Title = dto.Title,
					Director = dto.Director,
					Year = dto.Year,
					Genre = dto.Genre,
					UserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!,
					PosterUrl = dto.PosterUrl
				};
				dbContext.Movies.Add(movie);

				var tags = await dbContext.Tags.Where(x => dto.Tags.Contains(x.Id)).ToListAsync();
				if (tags.Count == 0)
				{
					return Results.NotFound();
				}

				foreach (var tag in tags)
				{
					movie.Tags.Add(tag);
				}

				dbContext.Movies.Add(movie);
				await dbContext.SaveChangesAsync();
				return Results.Created($"/api/movies/{movie.Id}", movie.ToDto());
			});
			moviesGroup.MapPut("/movies/{movieId:int}", async (MoviesDbContext dbContext, int movieId, UpdateCreateMovieDto dto) =>
			{
				var movie = await dbContext.Movies.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == movieId);
				var tags = await dbContext.Tags.Where(x => dto.Tags.Contains(x.Id)).ToListAsync();
				if (movie == null || tags.Count == 0)
				{
					return Results.NotFound();
				}
				movie.Title = dto.Title;
				movie.Director = dto.Director;
				movie.Year = dto.Year;
				movie.Genre = dto.Genre;
				movie.Tags.Clear();
				foreach (var tag in tags)
				{
					movie.Tags.Add(tag);
				}

				dbContext.Movies.Update(movie);
				await dbContext.SaveChangesAsync();
				return Results.Ok(movie.ToDto());
			});
			moviesGroup.MapDelete("/movies/{movieId:int}", async (MoviesDbContext dbContext, int movieId) =>
			{
				var movie = await dbContext.Tags.SelectMany(x => x.Movies).FirstOrDefaultAsync(x => x.Id == movieId);
				if (movie == null)
				{
					return Results.NotFound();
				}

				dbContext.Movies.Remove(movie);
				await dbContext.SaveChangesAsync();
				return Results.NoContent();
			});

		}
	}
}
