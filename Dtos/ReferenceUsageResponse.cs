namespace AuthorIt.Dtos
{
    public record ReferenceUsageResponse(
        int ReferenceId,
        int Level,
        int Position
    );
}
