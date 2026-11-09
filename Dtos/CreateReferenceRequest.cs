using System.ComponentModel.DataAnnotations;

namespace AuthorIt.Dtos
{
    public record CreateReferenceRequest
    {
        [Required(ErrorMessage = "Select a valid topic.")]
        public int TopicId { get; init; }
        [Required(ErrorMessage = "Enter a valid level.")]
        public int Level { get; init; }
        public int? Position { get; init; } = null;
    }
}