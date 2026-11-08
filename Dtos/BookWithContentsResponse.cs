namespace AuthorIt.Dtos
{
    public record BookResponseWithContents(
        int Id,
        string Title,
        string Description,
        DateTime Created,
        IReadOnlyList<ReferenceResponse> Contents
    );
}
