namespace AuthorIt.Dtos
{
    public record ReferenceResponse(
        int ReferenceId,
        int TopicId,
        string TopicTitle,
        int Level,
        int Position
    );
}
