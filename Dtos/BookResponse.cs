namespace AuthorIt.Dtos
{
    public record BookResponse(
        int Id,
        string Title,
        string Description,
        DateTime Created
    );
}
