using AuthorIt.Dtos;
using AuthorIt.Models;

namespace AuthorIt.Mappings
{
    public static class BookMappings
    {
        public static BookResponse ToResponse(this Book book)
        {
            return new BookResponse(
                book.Id,
                book.Title,
                book.Description,
                book.Created
            );
        }

    }
}
