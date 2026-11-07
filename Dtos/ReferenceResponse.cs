namespace AuthorIt.Dtos
{
    public record ReferenceResponse(
        int ReferenceId,
        int TopicId,
        int TopicTitle,
        int Level,
        int Position
    );
}
