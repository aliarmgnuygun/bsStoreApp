using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.Contracts;

namespace WebApi.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IRepositoryManager _manager;

        public BooksController(IRepositoryManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _manager.Book.GetAllBooks(trackChanges: false);
                return Ok(books);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        [HttpGet("{id:int}")]
        public IActionResult GetBookById([FromRoute(Name = "id")] int id)
        {
            var book = _manager
                .Book
                .GetOneBookById(id, trackChanges: false);

            if (book is null)
            {
                return NotFound(); // 404
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            try
            {
                if (book is null)
                {
                    return BadRequest(); // 400
                }

                _manager.Book.CreateOneBook(book);
                _manager.Save();
                return StatusCode(201, book);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateBook([FromRoute(Name = "id")] int id, [FromBody] Book book)
        {

            try
            {

                var entity = _manager.Book
                    .GetOneBookById(id, trackChanges: true);

                if (entity is null)
                    return NotFound(); // 404

                if (id != book.Id)
                    return BadRequest();

                entity.Title = book.Title;
                entity.Price = book.Price;

                _manager.Book.UpdateOneBook(entity);
                _manager.Save();
                return Ok(book);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteBookById([FromRoute(Name = "id")] int id)
        {
            var entity = _manager
                .Book
                .GetOneBookById(id, trackChanges: false);

            if (entity is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Book with id {id} could not found"
                });
            }

            _manager.Book.DeleteOneBook(entity);
            _manager.Save();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            var entity = _manager
                .Book
                .GetOneBookById(id, trackChanges: true);

            if (entity is null)
                return NotFound(); // 404

            bookPatch.ApplyTo(entity);
            _manager.Book.UpdateOneBook(entity);
            _manager.Save();

            return NoContent();
        }
    }
}
