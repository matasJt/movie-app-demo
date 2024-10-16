using System.Data;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MovieAppAPI.Data;
using MovieAppAPI.Data.Dtos;
using MovieAppAPI.Data.Entities;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<MoviesDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();
app.Use(async (context, next) =>
{
	await next();

	if (context.Response.StatusCode == StatusCodes.Status404NotFound)
	{
		if (context.Request.Path.StartsWithSegments("/api/tags") && !context.Request.Path.Value.Contains("/movies"))
		{
			await context.Response.HttpContext.Response.WriteAsync("Tag was not found");
		
		}
		else if (context.Request.Path.StartsWithSegments("/api/movies") && !context.Request.Path.Value.Contains("/reviews"))
		{
			await context.Response.HttpContext.Response.WriteAsync("Movie was not found");
		}
		else if (context.Request.Path.Value.Contains("/reviews"))
		{
			await context.Response.HttpContext.Response.WriteAsync("Review was not found");
		}
		else
		{
			await context.Response.WriteAsync("URL was not found");
		}
		
	}
});
var tagsGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();
tagsGroup.MapGet("/tags", async (MoviesDbContext dbContext) =>
{
	return (await dbContext.Tags.ToListAsync()).Select(x => x.ToDto());
});
tagsGroup.MapGet("/tags/{tagId:int}", async (MoviesDbContext dbContext, int tagId) =>
{
	var tag = await dbContext.Tags.FirstOrDefaultAsync(x => x.Id == tagId);

	if (tag == null)
	{
		return Results.NotFound();
	}
	return Results.Ok( new TagDto(tag.Id, tag.Title));
});
tagsGroup.MapPost("/tags", async (MoviesDbContext dbContext, CreateUpdateTagDto dto) =>
{
	var tag = new Tag {Title=dto.Title, Description=dto.Description};

	dbContext.Tags.Add(tag);
	await dbContext.SaveChangesAsync();
	return Results.Created($"/api/tags/{tag.Id}", tag.ToDto());
});
tagsGroup.MapPut("/tags/{tagId:int}", async (MoviesDbContext dbContext, int tagId, CreateUpdateTagDto dto ) =>
{
	var tag = await dbContext.Tags.FindAsync(tagId);

	if (tag == null)
	{
		return Results.NotFound();
	}
	
	tag.Title = dto.Title;
	tag.Description = dto.Description;

	dbContext.Tags.Update(tag);
	await dbContext.SaveChangesAsync();
	return Results.Ok(tag.ToDto());

});
tagsGroup.MapDelete("/tags/{tagId:int}", async (MoviesDbContext dbContext, int tagId) =>
{
	var tag = await dbContext.Tags.FindAsync(tagId);
	if (tag == null)
	{
		return Results.NotFound();
	}
	dbContext.Tags.Remove(tag);
	await dbContext.SaveChangesAsync();
	return Results.NoContent();
});

var moviesGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();

moviesGroup.MapGet("/tags/{tagId:int}/movies", async (MoviesDbContext dbContext,int tagId) =>
{
	return (await dbContext.Tags.Where(x => x.Id == tagId).SelectMany(x => x.Movies).ToListAsync()).Select(x => x.ToDto());

});
moviesGroup.MapGet("/movies/{movieId:int}", async (MoviesDbContext dbContext, int movieId) =>
{
	var movie = await dbContext.Movies.Include(x=> x.Tags).FirstOrDefaultAsync(x => x.Id == movieId);
	if (movie == null)
	{
		return Results.NotFound();
	}

	return Results.Ok(movie.ToDto());
});
moviesGroup.MapPost("/movies", async (MoviesDbContext dbContext,UpdateCreateMovieDto dto) =>
{
	var movie = new Movie
	{
		Title = dto.Title,
		Director = dto.Director,
		Year = dto.Year,
		Genre = dto.Genre
	};
	dbContext.Movies.Add(movie);
	
	var tags = await dbContext.Tags.Where(x=> dto.Tags.Contains(x.Id)).ToListAsync();
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
	foreach(var tag in tags)
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

var reviewsGroup = app.MapGroup("/api/").AddFluentValidationAutoValidation();

reviewsGroup.MapGet("movies/{movieId:int}/reviews", async (MoviesDbContext dbContext,int movieId) =>
{
	return (await dbContext.Reviews.Where(x=> x.Movie.Id == movieId).ToListAsync()).Select(x => x.ToDto());
});
reviewsGroup.MapGet("tags/{tagId:int}/movies/{movieId:int}/reviews/{reviewId:int}", async (MoviesDbContext dbContext, int reviewId, int movieId,int tagId) =>
{
	var review = await dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId && x.Movie.Id == movieId);
	var movie = await dbContext.Tags.Where(x => x.Id == tagId).SelectMany(x => x.Movies).FirstOrDefaultAsync(x => x.Id == movieId);
	if (review == null || movie == null)
	{
		return Results.NotFound();
	}
	return Results.Ok(review.ToDto());
});
reviewsGroup.MapPost("movies/{movieId:int}/reviews", async (MoviesDbContext dbContext, int movieId,CreateUpdateReviewDto dto) =>
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
		Movie = movie

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
app.MapControllers();
app.Run();