using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MovieAppAPI.Auth.Model;
using MovieAppAPI.Data.Dtos;
using MovieAppAPI.Data.Entities;
using MovieAppAPI.Data;

namespace MovieAppAPI.Endpoints
{
	public class TagEndpoints
	{
		public static void MapTagEndpoints(RouteGroupBuilder tagsGroup)
		{
			
			tagsGroup.MapGet("/tags", async  (MoviesDbContext dbContext ) =>
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

				return Results.Ok(new TagDto(tag.Id, tag.Title));
			});
			tagsGroup.MapPost("/tags", async (MoviesDbContext dbContext, CreateUpdateTagDto dto, HttpContext httpContext) =>
			{
				var tag = new Tag { Title = dto.Title, Description = dto.Description, UserId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)! };

				dbContext.Tags.Add(tag);
				await dbContext.SaveChangesAsync();
				return Results.Created($"/api/tags/{tag.Id}", tag.ToDto());
			});
			tagsGroup.MapPut("/tags/{tagId:int}", async (MoviesDbContext dbContext, int tagId, CreateUpdateTagDto dto) =>
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
		}
	}
}
