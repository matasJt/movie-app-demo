using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieAppAPI.Data.Dtos;
using MovieAppAPI.Data.Entities;
using MovieAppAPI.Data;
using Microsoft.IdentityModel.JsonWebTokens;
using MovieAppAPI.Auth.Model;
using Microsoft.AspNetCore.Http;

namespace MovieAppAPI.Endpoints
{
	public class ReviewEndpoints
	{
		public static void MapReviewEndpoints(RouteGroupBuilder reviewsGroup)
		{
			reviewsGroup.MapGet("movies/{movieId:int}/reviews", async (MoviesDbContext dbContext, int movieId, UserManager<User> usermanager) =>
			{
				var reviews = await dbContext.Reviews
					.Where(r => r.Movie.Id == movieId)
					.Include(r => r.User)
					.ToListAsync();

				var reviewDtos = reviews.Select(r => new ReviewDto(
					r.Id,
					r.Content,
					r.Rating,
					r.CreatedAt,
					r.User?.UserName ?? "Unknown",
					r.UserId
				)).ToList();
				return Results.Ok(reviewDtos);
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
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
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
					UserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!,
					User = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
				};

				dbContext.Reviews.Add(review);
				await dbContext.SaveChangesAsync();
				return Results.Created($"/api/{movieId}/reviews/{review.Id}", review.ToDto());

			});
			reviewsGroup.MapPut("movies/{movieId:int}/reviews/{reviewId:int}", async (MoviesDbContext dbContext, CreateUpdateReviewDto dto, int movieId, int reviewId, HttpContext httpContext) =>
			{
				var review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId && x.Movie.Id == movieId);

				if (review == null)
				{
					return Results.NotFound();
				}
				var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
				review.Content = dto.Content;
				review.Rating = dto.Rating;
				review.UserId = userId;
				review.User = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

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
