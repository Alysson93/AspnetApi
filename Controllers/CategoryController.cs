using System.Security.Claims;
using AspnetApi.Data;
using AspnetApi.Domain.Products;
using AspnetApi.Dtos;
using AspnetApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/categories")]
public class CategoryController
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _http;

    public CategoryController(AppDbContext context, IHttpContextAccessor http)
    {
        _context = context;
        _http = http;
    }

    [HttpGet] [AllowAnonymous]
    public async Task<IResult> Get([FromQuery]int skip = 0, [FromQuery]int take = 5)
    {
        var categories = await _context.Categories.Skip(skip).Take(take).ToListAsync();
        var response = categories.Select(c => new CategoryResponse(c.Name, c.Active));
        return Results.Ok(response);
    }

    [HttpGet("{id:guid}")] [AllowAnonymous]
    public async Task<IResult> GetBy([FromRoute] Guid id)
    {
        var category = await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        if (category == null) return Results.NotFound();
        var response = new CategoryResponse(category.Name, category.Active);
        return Results.Ok(response);
    }

    [HttpPost]
    public async Task<IResult> Post([FromBody] CategoryRequest request)
    {
        var userId = _http.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        Category category = new Category(request.Name, userId);
        if (!category.IsValid)
            return Results.ValidationProblem(category.Notifications.ConvertToProblemDetails());
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return Results.Created($"/api/v1/categories/{category.Id}", category.Id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> Put([FromRoute] Guid id, [FromBody] CategoryRequest request)
    {
        var userId = _http.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var category = await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        if (category == null) return Results.NotFound();
        category.Edit(request.Name, request.Active, userId);
        await _context.SaveChangesAsync();
        return Results.NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete([FromRoute] Guid id)
    {
        var category = await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        if (category == null) return Results.NotFound();
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Results.NoContent();
    }
}
