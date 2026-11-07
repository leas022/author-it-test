using System.ComponentModel.DataAnnotations;

namespace AuthorIt.Dtos
{
    public record CreateBookRequest
    {
        [Required]
        public string Title { get; init; } = "";
        public string Description { get; init; } = "";
    }
}
