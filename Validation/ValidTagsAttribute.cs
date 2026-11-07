using System.ComponentModel.DataAnnotations;

namespace AuthorIt.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public sealed class ValidTagsAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not IEnumerable<string> tags)
            {
                return new ValidationResult("Tags must be a collection of strings.");
            }

            var tagList = tags.ToList();

            if (tagList.Count == 0)
            {
                return ValidationResult.Success;
            }

            if (tagList.Any(t => string.IsNullOrWhiteSpace(t)))
            {
                return new ValidationResult("Tags cannot be empty.");
            }

            if (tagList.Distinct(StringComparer.OrdinalIgnoreCase).Count() != tagList.Count)
            {
                return new ValidationResult("Tags must be unique.");
            }

            if (tagList.Any(t => t.Length > 50))
            {
                return new ValidationResult("Tags cannot exceed 50 characters.");
            }

            return ValidationResult.Success;
        }
    }
}
