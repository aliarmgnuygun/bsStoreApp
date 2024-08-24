using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            var books = _manager.BookService.GetAllBooks(trackChanges: false);
            return Ok(books);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            throw new Exception("An error occurred while processing your request. Please try again later.");

            var book = _manager
                .BookService
                .GetBookById(id, trackChanges: false);

            if (book is null)
            {
                return NotFound(); // 404
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            if (book is null)
            {
                return BadRequest(); // 400
            }

            _manager.BookService.CreateOneBook(book);

            return StatusCode(201, book);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateBook([FromRoute(Name = "id")] int id, [FromBody] Book book)
        {
            if (book is null)
            {
                return BadRequest(); // 400
            }

            _manager.BookService.UpdateOneBook(id, book, trackChanges: true);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBookById([FromRoute(Name = "id")] int id)
        {
            _manager.BookService.DeleteOneBook(id, trackChanges: false);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            var entity = _manager
                .BookService.GetBookById(id, trackChanges: true);

            if (entity is null)
                return NotFound(); // 404

            bookPatch.ApplyTo(entity);
            _manager.BookService.UpdateOneBook(id, entity, trackChanges: true);

            return NoContent();
        }
    }
}
