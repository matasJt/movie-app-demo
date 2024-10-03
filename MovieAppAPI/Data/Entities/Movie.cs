using MovieAppAPI.Data.Dtos;

namespace MovieAppAPI.Data.Entities
{
	public class Movie
	{
		public int Id { get; set; }
		public required string Title { get; set; }
		public required string Director { get; set; }
		public required int Year { get; set; }
		public required string Genre { get; set; }
		public List<Tag> Tags { get; set; } = new List<Tag>();

		public MovieDto ToDto()
		{
			return new MovieDto(Title, Director, Year, Genre);
		}
	}
}
