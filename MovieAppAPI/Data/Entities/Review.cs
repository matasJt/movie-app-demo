using MovieAppAPI.Data.Dtos;

namespace MovieAppAPI.Data.Entities
{
	public class Review
	{
		public int Id { get; set; }
		public required string Content { get; set; }
		public required int Rating { get; set; }
		public required DateTimeOffset CreatedAt { get; set; }
		public Movie Movie { get; set; }

		public ReviewDto ToDto()
		{
			return new ReviewDto(Content, Rating, CreatedAt);
		}
	}
}
