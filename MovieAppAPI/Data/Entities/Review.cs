using MovieAppAPI.Auth.Model;
using MovieAppAPI.Data.Dtos;
using System.ComponentModel.DataAnnotations;

namespace MovieAppAPI.Data.Entities
{
	public class Review
	{
		public int Id { get; set; }
		public required string Content { get; set; }
		public required int Rating { get; set; }
		public required DateTimeOffset CreatedAt { get; set; }
		public Movie Movie { get; set; }
		[Required]
		public required string UserId { get; set; }
		public User? User { get; set; }

		public ReviewDto ToDto()
		{
			return new ReviewDto(Id, Content, Rating, CreatedAt,User.UserName,UserId);
		}
	}
}
