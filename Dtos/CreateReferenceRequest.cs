using System.ComponentModel.DataAnnotations;

namespace AuthorIt.Dtos
{
    public record CreateReferenceRequest
    {
        [Required]
        public int TopicId { get; init; }
        [Required]
        public int Level { get; init; }
        public int? Position { get; init; } = null;
    }
}