using AuthorIt.Data;
using AuthorIt.Dtos;
using AuthorIt.Models;
using AuthorIt.Services;

namespace AuthorIt.Tests;

public class BooksServiceTests
{
    [Fact]
    public void CreateReference_WhenTopicDoesNotExist_RejectsWithoutChangingBook()
    {
        var store = new LibraryStore();
        var service = new BooksService(store);
        var before = service.GetById(1)!.Contents.ToArray();

        var result = service.CreateReference(1, new CreateReferenceRequest
        {
            TopicId = int.MaxValue,
            Level = 1,
            Position = 1
        });

        Assert.Equal(BooksService.CreateReferenceStatus.TopicNotFound, result.Status);
        Assert.Null(result.Book);
        Assert.Equal(before, service.GetById(1)!.Contents.ToArray());
    }

    [Fact]
    public void CreateReference_WhenHierarchyIsInvalid_RejectsWithoutChangingBook()
    {
        var store = new LibraryStore();
        var service = new BooksService(store);
        var before = service.GetById(1)!.Contents.ToArray();

        // The preceding entry is level 1, so jumping to level 3 is invalid.
        var result = service.CreateReference(1, new CreateReferenceRequest
        {
            TopicId = 5,
            Level = 3,
            Position = 2
        });

        Assert.Equal(BooksService.CreateReferenceStatus.InvalidHierarchy, result.Status);
        Assert.Null(result.Book);
        Assert.Equal(before, service.GetById(1)!.Contents.ToArray());
    }
}
