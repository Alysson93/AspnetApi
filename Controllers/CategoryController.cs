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

    [HttpPost]
    public IResult Post([FromBody] CategoryRequest request)
    {
        Category category = new Category
        {
            Name = request.Name,
            CreatedBy = "Test User",
            EditedBy = "Test User"
        };
        _context.Categories.Add(category);
        _context.SaveChanges();
        return Results.Created($"/api/v1/categories/{category.Id}", category.Id);
    }
}
