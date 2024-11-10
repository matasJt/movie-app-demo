using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieAppAPI.Data.Dtos;
using MovieAppAPI.Data.Entities;
using MovieAppAPI.Data;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MovieAppAPI.Endpoints
{
	public class ReviewEndpoints
	{
		public static void MapReviewEndpoints(RouteGroupBuilder reviewsGroup)
		{
			reviewsGroup.MapGet("movies/{movieId:int}/reviews", async (MoviesDbContext dbContext, int movieId) =>
			{
				return (await dbContext.Reviews.Where(x => x.Movie.Id == movieId).ToListAsync()).Select(x => x.ToDto());
			});
			reviewsGroup.MapGet("tags/{tagId:int}/movies/{movieId:int}/reviews/{reviewId:int}", async (MoviesDbContext dbContext, int reviewId, int movieId, int tagId) =>
			{
				var review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId && x.Movie.Id == movieId);
				var movie = await dbContext.Tags.Where(x => x.Id == tagId).SelectMany(x => x.Movies).FirstOrDefaultAsync(x => x.Id == movieId);
				if (review == null || movie == null)
				{
					return Results.NotFound();
				}
				return Results.Ok(review.ToDto());
			});
			reviewsGroup.MapPost("movies/{movieId:int}/reviews", async (MoviesDbContext dbContext, int movieId, CreateUpdateReviewDto dto, HttpContext httpContext) =>
			{
				var movie = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == movieId);
				if (movie == null)
				{
					return Results.NotFound();
				}
				var review = new Review
				{
					Content = dto.Content,
					Rating = dto.Rating,
					CreatedAt = DateTimeOffset.UtcNow,
					Movie = movie,
					UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!
				};

				dbContext.Reviews.Add(review);
				await dbContext.SaveChangesAsync();
				return Results.Created($"/api/{movieId}/reviews/{review.Id}", review.ToDto());

			});
			reviewsGroup.MapPut("movies/{movieId:int}/reviews/{reviewId:int}", async (MoviesDbContext dbContext, CreateUpdateReviewDto dto, int movieId, int reviewId) =>
			{
				var review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId && x.Movie.Id == movieId);

				if (review == null)
				{
					return Results.NotFound();
				}
				review.Content = dto.Content;
				review.Rating = dto.Rating;

				dbContext.Reviews.Update(review);
				await dbContext.SaveChangesAsync();
				return Results.Ok(review.ToDto());
			});
			reviewsGroup.MapDelete("movies/{movieId:int}/reviews/{reviewId:int}", async (MoviesDbContext dbContext, int movieId, int reviewId) =>
			{
				var review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId && x.Movie.Id == movieId);
				if (review == null)
				{
					return Results.NotFound();
				}

				dbContext.Remove(review);
				await dbContext.SaveChangesAsync();
				return Results.NoContent();
			});
		}
	}
}
