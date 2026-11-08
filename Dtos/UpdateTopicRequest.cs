using AuthorIt.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuthorIt.Dtos
{
    public record UpdateTopicRequest : CreateTopicRequest
    {
        [Required]
        public int Version { get; init; }
    }
}
