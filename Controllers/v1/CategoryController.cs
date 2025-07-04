using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;

namespace SampleWebApiAspNetCore.Controllers.v1;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly FoodDbContext _context;

    public CategoryController(FoodDbContext context)
    {
        _context = context;
    }

    // GET: api/category
    [HttpGet]
    public ActionResult<IEnumerable<CategoryReadDto>> GetCategories()
    {
        var categories = _context.Categories
            .Select(c => new CategoryReadDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToList();

        return Ok(categories);
    }

    // GET: api/category/5
    [HttpGet("{id}")]
    public ActionResult<CategoryReadDto> GetCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
            return NotFound();

        return Ok(new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name
        });
    }

    // POST: api/category
    [HttpPost]
    public ActionResult<CategoryReadDto> CreateCategory(CategoryCreateDto dto)
    {
        var category = new CategoryEntity
        {
            Name = dto.Name
        };

        _context.Categories.Add(category);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new CategoryReadDto
        {
            Id = category.Id,
            Name = category.Name
        });
    }

    // PUT: api/category/5
    [HttpPut("{id}")]
    public IActionResult UpdateCategory(int id, CategoryCreateDto dto)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
            return NotFound();

        category.Name = dto.Name;

        _context.SaveChanges();
        return NoContent();
    }

    // DELETE: api/category/5
    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category == null)
            return NotFound();

        _context.Categories.Remove(category);
        _context.SaveChanges();
        return NoContent();
    }
}
