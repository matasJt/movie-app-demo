using System.Data;
using FluentValidation;
using MovieAppAPI.Data.Entities;

namespace MovieAppAPI.Data.Dtos
{
	public record TagDto(int Id, string Title);

	public record CreateUpdateTagDto(string Title, string Description)
	{
		public class Validator : AbstractValidator<CreateUpdateTagDto>
		{
			public Validator()
			{
				RuleFor(x => x.Title).NotEmpty().Length(2, 20);
				RuleFor(x => x.Description).NotEmpty().Length(10, 50);
			}
		}
	}

	public record MovieDto(int Id, string Title, string Director, int Year, string Genre, List<string> TagNames, string PosterUrl);
	public record UpdateCreateMovieDto(string Title, string Director, int Year, string Genre, List<int> Tags, string PosterUrl)
	{
		public class Validator : AbstractValidator<UpdateCreateMovieDto>
		{
			public Validator()
			{
				RuleFor(x => x.Title).NotEmpty().Length(2, 100);
				RuleFor(x => x.Director).NotEmpty().Length(2, 100);
				RuleFor(x => x.Year).GreaterThan(1900);
				RuleFor(x => x.Genre).NotEmpty();
			}
		}
	}

	public record ReviewDto(int Id, string Content, int Rating, DateTimeOffset CreatedAt, string UserName,string UserId);

	public record CreateUpdateReviewDto(string Content, int Rating)
	{
		public class Validator : AbstractValidator<CreateUpdateReviewDto>
		{
			public Validator()
			{
				RuleFor(x => x.Content).NotEmpty();
				RuleFor(x => x.Rating).GreaterThan(-1).LessThan(11);
			}
		}
	}
}
