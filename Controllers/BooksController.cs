using AuthorIt.Dtos;
using AuthorIt.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthorIt.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BooksService _booksService;
        public BooksController(BooksService booksService)
        {
            _booksService = booksService;
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<BookResponse>> GetAll()
        {
            return Ok(_booksService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<BookResponseWithContents> GetById(int id)
        {
            var book = _booksService.GetById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        public ActionResult<BookResponseWithContents> Create([FromBody] CreateBookRequest request)
        {
            var book = _booksService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPost("{id}/references")]
        public ActionResult<BookResponseWithContents> CreateReference(int id, [FromBody] CreateReferenceRequest request)
        {
            var result = _booksService.CreateReference(id, request);

            switch (result.Status)
            {
                case BooksService.CreateReferenceStatus.BookNotFound:
                    return NotFound("Book does not exist.");
                case BooksService.CreateReferenceStatus.TopicNotFound:
                    return NotFound("Topic does not exist.");
                case BooksService.CreateReferenceStatus.InvalidPosition:
                    return BadRequest("Invalid position for the reference.");
                case BooksService.CreateReferenceStatus.InvalidHierarchy:
                    return BadRequest("Invalid hierarchy for the reference.");
            }

            return CreatedAtAction(nameof(GetById), new { id }, result.Book);
        }

        [HttpDelete("{id}/references/{referenceId}")]
        public ActionResult<BookResponseWithContents> DeleteReference(int id, int referenceId)
        {
            var result = _booksService.DeleteReference(id, referenceId);

            return result.Status switch
            {
                BooksService.DeleteReferenceStatus.BookNotFound => NotFound("Book does not exist."),
                BooksService.DeleteReferenceStatus.ReferenceNotFound => NotFound("Reference does not exist in this book."),
                _ => Ok(result.Book)
            };
        }


    }
}
