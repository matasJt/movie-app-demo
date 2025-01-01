using MovieAppAPI.Auth.Model;
using MovieAppAPI.Data.Dtos;
using System.ComponentModel.DataAnnotations;

namespace MovieAppAPI.Data.Entities
{
	public class Movie
	{
		public int Id { get; set; }
		public required string Title { get; set; }
		public required string Director { get; set; }
		public required int Year { get; set; }
		public required string Genre { get; set; }
		public required string Poster { get; set; }
		public required string Plot { get; set; }
		public required string Country { get; set; }
		public required string Actors { get; set; }
		public required string Runtime { get; set; }
		public List<Tag> Tags { get; set; } = new List<Tag>();
		[Required]
		public required string UserId { get; set; }
		
		public User? User { get; set; }
		public MovieDto ToDto()
		{
			return new MovieDto(Id, Title, Director, Year, Genre,Tags.Select(x=> x.Title).ToList(), Tags.Select(x=> x.Id).ToList(), Poster, Plot, Country, Actors, Runtime);
		}
	}
}
