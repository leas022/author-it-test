using AuthorIt.Dtos;
using AuthorIt.Models;

namespace AuthorIt.Mappings
{
    public static class TopicMappings
    {
        public static TopicResponse ToResponse(this Topic topic)
        {
            return new TopicResponse(
                topic.Id,
                topic.Title,
                topic.Body,
                topic.Tags.AsReadOnly(),
                topic.Created,
                topic.Updated,
                topic.Version
            );
        }

    }
}
