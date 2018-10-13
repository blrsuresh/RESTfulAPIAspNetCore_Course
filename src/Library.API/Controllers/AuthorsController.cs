using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

[Route("api/authors")]
public class AuthorsController : Controller
{
    private ILibraryRepository _libraryRepository;
    public AuthorsController(ILibraryRepository libraryRepository)
    {
        this._libraryRepository = libraryRepository;
    }
    [HttpGet]
    public IActionResult GetAuthors()
    {
        var authorsfromRepo = _libraryRepository.GetAuthors();
        var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsfromRepo);
        //foreach (var author in authorsfromRepo)
        //{
        //    authors.Add(new AuthorDto()
        //    {
        //        Id = author.Id,
        //        Name = $"{author.FirstName} {author.LastName}",
        //        Genere = author.Genre,
        //        Age = author.DateOfBirth.GetCurrentAge(),
        //    });

        //}

        return Ok(authors);
    }

    [HttpGet("{id}", Name = "GetAuthor")]
    public IActionResult GetAuthor(Guid id)
    {
        var author = _libraryRepository.GetAuthor(id);
        if (author == null)
        {
            return NotFound();
        }

        var result = Mapper.Map<AuthorDto>(author);
        return Ok(author);
    }

    [HttpPost]
    public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
    {
        if (author == null)
        {
            return BadRequest();
        }

        var authorEntity = Mapper.Map<Author>(author);

        _libraryRepository.AddAuthor(authorEntity);

        if (!_libraryRepository.Save())
        {
            throw new Exception("Creating an author failed on save");
        }

        var authortoReturn = Mapper.Map<AuthorDto>(authorEntity);

        return CreatedAtRoute("GetAuthor", new { id = authortoReturn.Id }, authortoReturn);

    }
}