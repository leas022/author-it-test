using AuthorIt.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AuthorIt.Dtos
{
    public record CreateTopicRequest
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot be more than 200 characters.")]
        public string Title { get; init; } = "";
        public string Body { get; init; } = "";
        [ValidTags]
        public List<string> Tags { get; init; } = [];
    }
}
