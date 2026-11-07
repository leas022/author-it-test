namespace AuthorIt.Dtos
{
    public record BookResponseWithContents(
        int Id,
        string Description,
        DateTime Created,
        IReadOnlyList<ReferenceResponse> Contents
    );
}
