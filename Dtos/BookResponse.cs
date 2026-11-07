namespace AuthorIt.Dtos
{
    public record BookResponse(
        int Id,
        string Description,
        DateTime Created
    );
}
