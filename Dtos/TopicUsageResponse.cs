namespace AuthorIt.Dtos
{
    public record TopicUsageResponse(
        int TopicId,
        string TopicTitle,
        IReadOnlyList<BookTopicUsageResponse> Books,
        int TotalReferences,
        int BookCount
    );

    public record BookTopicUsageResponse(
        int BookId,
        string BookTitle,
        string BookDescription,
        DateTime BookCreated,
        IReadOnlyList<ReferenceUsageResponse> References
);
}
