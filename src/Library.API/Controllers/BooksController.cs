using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

[Route("api/authors/{authorid}/books")]
public class BooksController : Controller
{
    private ILibraryRepository _libraryRepository;
    public BooksController(ILibraryRepository libraryRepository)
    {
        this._libraryRepository = libraryRepository;
    }

    [HttpGet()]
    public IActionResult GetBooksForAuthor(Guid authorId)
    {
        if (!_libraryRepository.AuthorExists(authorId))
        {
            return NotFound();
        }

        var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);
        var booksForAuthor = Mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo);

        return Ok(booksForAuthor);
    }

    [HttpGet("{Id}", Name = "GetBookForAuthor")]
    public IActionResult GetBooksForAuthor(Guid authorId, Guid id)
    {
        if (!_libraryRepository.AuthorExists(authorId))
        {
            return NotFound();
        }

        var getBookforAuthor = _libraryRepository.GetBookForAuthor(authorId, id);
        if (getBookforAuthor == null)
        {
            return NotFound();
        }

        var bookForAuthor = Mapper.Map<BookDto>(getBookforAuthor);

        return Ok(bookForAuthor);
    }

    public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto book)
    {
        if (book == null)
        {
            return BadRequest();
        }

        if (!_libraryRepository.AuthorExists(authorId))
        {
            return NotFound();
        }

        var bookEntity = Mapper.Map<Book>(book);

        _libraryRepository.AddBookForAuthor(authorId, bookEntity);

        if (!_libraryRepository.Save())
        {
            throw new Exception($"Creating a book for author {authorId} failed on the server");
        }

        var booktoReturn = Mapper.Map<BookDto>(bookEntity);

        return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = booktoReturn.Id }, booktoReturn);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
    {
        if(!_libraryRepository.AuthorExists(authorId))
        {
            return NotFound();
        }

        var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
        if (bookForAuthorFromRepo == null)
        {
            return NotFound();
        }

        _libraryRepository.DeleteBook(bookForAuthorFromRepo);

        if(!_libraryRepository.Save())
        {
            throw new Exception($"Deleting book {id} for author {authorId} failed on save");
        }

        return NoContent();
    }
}
