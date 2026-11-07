using AuthorIt.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuthorIt.Dtos
{
    public record UpdateTopicRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; init; } = "";
        public string Body { get; init; } = "";
        [ValidTags]
        public List<string> Tags { get; init; } = [];
        [Required]
        public int Version { get; init; }
    }
}
