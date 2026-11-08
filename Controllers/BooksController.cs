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
        [EndpointName("getBooks")]
        [ProducesResponseType(typeof(IReadOnlyList<BookResponse>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<BookResponse>> GetAll()
        {
            return Ok(_booksService.GetAll());
        }

        [HttpGet("{id}")]
        [EndpointName("getBookById")]
        [ProducesResponseType(typeof(BookResponseWithContents), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<BookResponseWithContents> GetById(int id)
        {
            var book = _booksService.GetById(id);
            if (book == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound,
                    title: "Book not found", detail: "The requested book does not exist.");
            }
            return Ok(book);
        }

        [HttpPost]
        [EndpointName("createBook")]
        [ProducesResponseType(typeof(BookResponseWithContents), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<BookResponseWithContents> Create([FromBody] CreateBookRequest request)
        {
            var book = _booksService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPost("{id}/references")]
        [EndpointName("createReference")]
        [ProducesResponseType(typeof(BookResponseWithContents), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<BookResponseWithContents> CreateReference(int id, [FromBody] CreateReferenceRequest request)
        {
            var result = _booksService.CreateReference(id, request);

            switch (result.Status)
            {
                case BooksService.CreateReferenceStatus.BookNotFound:
                    return Problem(statusCode: StatusCodes.Status404NotFound,
                        title: "Book not found", detail: "The requested book does not exist.");
                case BooksService.CreateReferenceStatus.TopicNotFound:
                    return Problem(statusCode: StatusCodes.Status404NotFound,
                        title: "Topic not found", detail: "The referenced topic does not exist.");
                case BooksService.CreateReferenceStatus.InvalidPosition:
                    ModelState.AddModelError("position",
                        "Position must be between 1 and the number of entries plus 1.");
                    return ValidationProblem(ModelState);
                case BooksService.CreateReferenceStatus.InvalidHierarchy:
                    ModelState.AddModelError("level",
                        "The level must be positive, the first entry must be level 1, " +
                        "and no entry may be more than one level deeper than its predecessor.");
                    return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id }, result.Book);
        }

        [HttpDelete("{id}/references/{referenceId}")]
        [EndpointName("deleteReference")]
        [ProducesResponseType(typeof(BookResponseWithContents), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<BookResponseWithContents> DeleteReference(int id, int referenceId)
        {
            var result = _booksService.DeleteReference(id, referenceId);

            return result.Status switch
            {
                BooksService.DeleteReferenceStatus.BookNotFound => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Book not found", detail: "The requested book does not exist."),
                BooksService.DeleteReferenceStatus.ReferenceNotFound => Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Reference not found", detail: "The reference does not exist in this book."),
                _ => Ok(result.Book)
            };
        }


    }
}
