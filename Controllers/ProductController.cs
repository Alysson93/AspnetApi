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
[Route("/products")]
public class ProductController
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _http;

    public ProductController(AppDbContext context, IHttpContextAccessor http)
    {
        _context = context;
        _http = http;
    }

    [HttpGet] [AllowAnonymous]
    public async Task<IResult> Get([FromQuery]int skip = 0, [FromQuery]int take = 5)
    {
        var products = await _context.Products.Skip(skip).Take(take).ToListAsync();
        var response = products.Select(p => new ProductResponse(p.Name, p.Category?.Name, p.Description, p.HasStock, p.Active));
        return Results.Ok(response);
    }

    [HttpPost]
    public async Task<IResult> Post([FromBody] ProductRequest request)
    {
        var userId = _http.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId);
        if (category == null)
            return Results.BadRequest("Category not found");
        Product product= new Product(request.Name, request.Description, category.Id, category, userId);
        if (!product.IsValid)
            return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return Results.Created($"/api/v1/products/{product.Id}", product.Id);
    }
}
