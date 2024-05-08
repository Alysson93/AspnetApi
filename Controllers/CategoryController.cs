using AspnetApi.Data;
using AspnetApi.Domain.Products;
using AspnetApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/api/v1/categories")]
public class CategoryController
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IResult Get([FromQuery]int skip = 0, [FromQuery]int take = 5)
    {
        var categories = _context.Categories.Skip(skip).Take(take).ToList();
        var response = categories.Select(c => new CategoryResponse(c.Name, c.Active));
        return Results.Ok(response);
    }

    [HttpGet("{id:guid}")]
    public IResult GetBy([FromRoute] Guid id)
    {
        var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        if (category == null) return Results.NotFound();
        var response = new CategoryResponse(category.Name, category.Active);
        return Results.Ok(response);
    }

    [HttpPost]
    public IResult Post([FromBody] CategoryRequest request)
    {
        Category category = new Category(request.Name, "Author");
        if (!category.IsValid)
        {
            var errors = category.Notifications.GroupBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Message)
                .ToArray());
            return Results.ValidationProblem(errors);
        }
        _context.Categories.Add(category);
        _context.SaveChanges();
        return Results.Created($"/api/v1/categories/{category.Id}", category.Id);
    }

    [HttpPut("{id:guid}")]
    public IResult Put([FromRoute] Guid id, [FromBody] CategoryRequest request)
    {
        var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        if (category == null) return Results.NotFound();
        category.Name = request.Name;
        category.Active = request.Active;
        category.EditedAt = DateTime.Now;
        category.EditedBy = "User edit";
        _context.SaveChanges();
        return Results.NoContent();
    }


    [HttpDelete("{id:guid}")]
    public IResult Delete([FromRoute] Guid id)
    {
        var category = _context.Categories.Where(c => c.Id == id).FirstOrDefault();
        if (category == null) return Results.NotFound();
        _context.Categories.Remove(category);
        _context.SaveChanges();
        return Results.NoContent();
    }
}
