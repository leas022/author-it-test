using AuthorIt.Data;
using AuthorIt.Dtos;
using AuthorIt.Mappings;
using AuthorIt.Models;

namespace AuthorIt.Services
{
    public class BooksService
    {
        public enum CreateReferenceStatus
        {
            Success,
            BookNotFound,
            TopicNotFound,
            InvalidPosition,
            InvalidHierarchy
        }
        public record CreateReferenceResponse(CreateReferenceStatus Status, BookResponseWithContents? Book = null);
        private readonly LibraryStore _libraryStore;

        public BooksService(LibraryStore libraryStore)
        {
            _libraryStore = libraryStore;
        }

        public IReadOnlyList<BookResponse> GetAll()
        {
            return _libraryStore.Books.Values.OrderBy(b => b.Id).Select(b => b.ToResponse()).ToList();
        }

        public BookResponseWithContents? GetById(int id)
        {
            _libraryStore.Books.TryGetValue(id, out var book);

            if (book == null)
            {
                return null;
            }

            var contents = book.Contents
                .Select((reference, Index) => new ReferenceResponse(
                    ReferenceId: reference.ReferenceId,
                    TopicId: reference.TopicId,
                    TopicTitle: _libraryStore.Topics[reference.TopicId].Title,
                    Level: reference.Level,
                    Position: Index + 1
                ))
                .ToList();

            return new BookResponseWithContents(
                Id: book.Id,
                Title: book.Title,
                Description: book.Description,
                Created: book.Created,
                Contents: contents
            );
        }

        public BookResponseWithContents Create(CreateBookRequest request)
        {
            var newBook = new Book
            { 
                Id = _libraryStore.GetNextBookId(),
                Title = request.Title,
                Description = request.Description,
            };

            _libraryStore.Books[newBook.Id] = newBook;

            return new BookResponseWithContents(
                Id: newBook.Id,
                Title: newBook.Title,
                Description: newBook.Description,
                Created: newBook.Created,
                Contents: []
            );
        }

        public CreateReferenceResponse CreateReference(int bookId, CreateReferenceRequest request)
        {
            if (!_libraryStore.Books.TryGetValue(bookId, out var book))
            {
                return new CreateReferenceResponse(CreateReferenceStatus.BookNotFound);
            }

            if (!_libraryStore.Topics.TryGetValue(request.TopicId, out var topic))
            {
                return new CreateReferenceResponse(CreateReferenceStatus.TopicNotFound);
            }

            var position = request.Position ?? book.Contents.Count + 1;
            if (position < 1 || position > book.Contents.Count + 1)
            {
                return new CreateReferenceResponse(CreateReferenceStatus.InvalidPosition);
            }

            var newReference = new Reference
            {
                TopicId = request.TopicId,
                Level = request.Level,
            };
            var proposed = book.Contents.ToList();
            proposed.Insert(position - 1, newReference);

            if (!IsValidHierarchy(proposed))
            {
                return new CreateReferenceResponse(CreateReferenceStatus.InvalidHierarchy);
            }

            book.Contents.Insert(position - 1, new Reference
            {
                ReferenceId = _libraryStore.GetNextReferenceId(),
                TopicId = request.TopicId,
                Level = request.Level
            });

            return new CreateReferenceResponse(CreateReferenceStatus.Success, GetById(bookId));
        }

        private static bool IsValidHierarchy(List<Reference> references)
        {
            for (int i = 0; i < references.Count; i++)
            {
                int level = references[i].Level;

                if (level < 1)
                {
                    return false;
                }


                if (i == 0 && level != 1)
                {
                    return false;
                }

                if (i > 0 && level > references[i - 1].Level + 1)
                {
                    return false;
                }

            }

            return true;
        }

        public enum DeleteReferenceStatus
        {
            Success,
            BookNotFound,
            ReferenceNotFound
        }

        public record DeleteReferenceResponse(DeleteReferenceStatus Status, BookResponseWithContents? Book = null);
        public DeleteReferenceResponse DeleteReference(int bookId, int referenceId)
        {
            if (!_libraryStore.Books.TryGetValue(bookId, out var book))
            {
                return new DeleteReferenceResponse(DeleteReferenceStatus.BookNotFound);
            }

            var startIndex = book.Contents.FindIndex(r => r.ReferenceId == referenceId);
            if (startIndex == -1)
            {
                return new DeleteReferenceResponse(DeleteReferenceStatus.ReferenceNotFound);
            }

            var parentLevel = book.Contents[startIndex].Level;
            var removeCount = 1;

            for (int i = startIndex + 1; i < book.Contents.Count; i++)
            {
                // Stop when sibling/ancestor level is reached.
                if (book.Contents[i].Level <= parentLevel)
                {
                    break;
                }

                removeCount++;
            }

            book.Contents.RemoveRange(startIndex, removeCount);

            return new DeleteReferenceResponse(DeleteReferenceStatus.Success, GetById(bookId));
        }
    }
}
