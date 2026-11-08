using AuthorIt.Data;
using AuthorIt.Dtos;
using AuthorIt.Mappings;
using AuthorIt.Models;

namespace AuthorIt.Services
{
    public class TopicsService
    {
        public record UpdateTopicResponse(bool TopicExists, TopicResponse? Topic);
        private readonly LibraryStore _libraryStore;

        public TopicsService(LibraryStore libraryStore)
        {
            _libraryStore = libraryStore;
        }

        public IReadOnlyList<TopicResponse> GetAll()
        {
            return _libraryStore.Topics.Values.OrderBy(t => t.Id).Select(t => t.ToResponse()).ToList();
        }

        public TopicResponse? GetById(int id)
        {
            _libraryStore.Topics.TryGetValue(id, out var topic);
            return topic?.ToResponse();
        }

        public TopicResponse Create(CreateTopicRequest request)
        {
            var newTopic = new Topic
            {
                Id = _libraryStore.GetNextTopicId(),
                Title = request.Title,
                Body = request.Body,
                Tags = request.Tags.ToList(),
                Created = DateTime.Now,
                Updated = DateTime.Now,
                Version = 1
            };
            _libraryStore.Topics[newTopic.Id] = newTopic;
            return newTopic.ToResponse();
        }

        public UpdateTopicResponse Update(int id, UpdateTopicRequest request)
        {
            if (!_libraryStore.Topics.TryGetValue(id, out var topic))
            {
                return new UpdateTopicResponse(false, null);
            }

            if (topic.Version != request.Version)
            {
                return new UpdateTopicResponse(true, null);
            }

            topic.Title = request.Title;
            topic.Body = request.Body;
            topic.Tags = request.Tags.ToList();
            topic.Updated = DateTime.Now;
            topic.Version++;

            return new UpdateTopicResponse(true, topic.ToResponse());
        }

        public TopicUsageResponse? GetTopicUsage(int id)
        {
            if (!_libraryStore.Topics.TryGetValue(id, out var topic))
            {
                return null;
            }

            var books = _libraryStore.Books.Values
                .Select(book =>
                {
                    var references = book.Contents
                        .Select((r, index) => new { Reference = r, Position = index + 1 })
                        .Where(x => x.Reference.TopicId == topic.Id)
                        .Select(x => new ReferenceUsageResponse(
                            ReferenceId: x.Reference.ReferenceId,
                            Level: x.Reference.Level,
                            Position: x.Position))
                        .ToList();

                    return new BookTopicUsageResponse(
                        BookId: book.Id,
                        BookTitle: book.Title,
                        BookDescription: book.Description,
                        BookCreated: book.Created,
                        References: references);
                })
                .Where(b => b.References.Count > 0)
                .ToList();

            return new TopicUsageResponse(
                TopicId: topic.Id,
                TopicTitle: topic.Title,
                Books: books,
                TotalReferences: books.Sum(b => b.References.Count),
                BookCount: books.Count);
        }


    }
}
