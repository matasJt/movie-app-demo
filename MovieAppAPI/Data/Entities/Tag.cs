using System.ComponentModel.DataAnnotations;
using MovieAppAPI.Auth.Model;
using MovieAppAPI.Data.Dtos;

namespace MovieAppAPI.Data.Entities
{
	public class Tag
	{
		public int Id { get; set; }
		public required string Title { get; set; }
		public required string Description { get; set; }
		public List<Movie> Movies { get; set; } = new List<Movie>();
		[Required]
		public required string UserId { get; set; }
		public User? User { get; set; }

		public TagDto ToDto()
		{
			return new TagDto(Id, Title, Description,UserId);
		}
	}
}
