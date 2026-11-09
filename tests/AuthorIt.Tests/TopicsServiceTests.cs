using AuthorIt.Data;
using AuthorIt.Dtos;
using AuthorIt.Services;

namespace AuthorIt.Tests;

public class TopicsServiceTests
{
    [Fact]
    public void Update_WhenVersionIsStale_RejectsWithoutChangingTopic()
    {
        var service = new TopicsService(new LibraryStore());
        var original = service.GetById(1)!;
        var firstUpdate = service.Update(1, new UpdateTopicRequest
        {
            Title = "Saved title",
            Body = "Saved body",
            Tags = ["saved"],
            Version = original.Version
        });
        Assert.True(firstUpdate.TopicExists);
        Assert.NotNull(firstUpdate.Topic);
        Assert.Equal(original.Version + 1, firstUpdate.Topic.Version);
        var before = service.GetById(1)!;

        var result = service.Update(1, new UpdateTopicRequest
        {
            Title = "Stale title",
            Body = "Stale body",
            Tags = ["stale"],
            Version = original.Version
        });

        Assert.True(result.TopicExists);
        Assert.Null(result.Topic);
        var after = service.GetById(1);
        Assert.NotNull(after);
        Assert.Equal(before.Id, after.Id);
        Assert.Equal(before.Title, after.Title);
        Assert.Equal(before.Body, after.Body);
        Assert.Equal(before.Tags.ToArray(), after.Tags.ToArray());
        Assert.Equal(before.Version, after.Version);
        Assert.Equal(before.Created, after.Created);
        Assert.Equal(before.Updated, after.Updated);
    }
}
