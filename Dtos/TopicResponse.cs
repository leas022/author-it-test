namespace AuthorIt.Dtos
{
    public record TopicResponse(
        int Id,
        string Title, 
        string Body, 
        IReadOnlyList<string> Tags,
        DateTime Created,
        DateTime Updated,
        int Version
    );
}
