using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Repositories.EFCore;

namespace WebApi.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly RepositoryContext _context;

        public BooksController(RepositoryContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetAllBooks()
        {
            try
            {
                var books = _context.Books.ToList();
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
            var book = _context
                .Books
                .Where(a => a.Id.Equals(id))
                .FirstOrDefault();

            if (book is null)
            {
                return NotFound(); // 404
            }

            return Ok(book);
        }

        [HttpPost]
        public IActionResult CreateBook([FromBody] Book book)
        {
            try
            {
                if (book is null)
                {
                    return BadRequest(); // 400
                }

                _context.Books.Add(book);
                _context.SaveChanges();
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

            try {

                var entity = _context
                .Books
                .Where(a => a.Id.Equals(id))
                .FirstOrDefault();

                if (entity is null)
                    return NotFound(); // 404

                if (id != book.Id)
                    return BadRequest();

                entity.Title = book.Title;
                entity.Price = book.Price;

                _context.SaveChanges();
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
            var entity = _context
                .Books
                .Where(a => a.Id.Equals(id))
                .SingleOrDefault();

            if (entity is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Book with id {id} could not found"
                });
            }

            _context.Books.Remove(entity);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            var entity = _context
                .Books
                .Where(a => a.Id.Equals(id))
                .SingleOrDefault();

            if (entity is null)
                return NotFound(); // 404

            bookPatch.ApplyTo(entity);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
