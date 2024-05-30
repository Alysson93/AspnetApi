using System.Security.Claims;
using AspnetApi.Data;
using AspnetApi.Dtos;
using AspnetApi.Services;
using AspnetApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/employees")]
public class EmployeeController : ControllerBase
{
    private readonly UserService _service;
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _http;

    public EmployeeController(
        UserService service, 
        AppDbContext context,
        IHttpContextAccessor http)
    {
        _service = service;
        _context = context;
        _http = http;
    }

    [HttpGet] [Authorize(Policy = "EmployeePolicy")]
    public async Task<IResult> Get([FromQuery] int skip = 0, [FromQuery] int take = 5)
    {
        var employees = await (
            from user in _context.Users
            join claim in _context.UserClaims
            on user.Id equals claim.UserId
            where claim.ClaimType == "Name"
            orderby claim.ClaimValue
            select new EmployeeResponse(user.Email, claim.ClaimValue)
        ).Skip(skip).Take(take).ToListAsync();
        return Results.Ok(employees);
    }

    [HttpPost] [Authorize(Policy = "AdminPolicy")]
    public async Task<IResult> Post(EmployeeRequest request)
    {
        var userId = _http.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userClaims = new List<Claim>
        {
            new Claim("Code", request.Code),
            new Claim("Name", request.Name),
            new Claim("CreatedBy", userId)
        };
        (IdentityResult result, string id) result = await _service.Create(request.Email, request.Password, userClaims);
        if (!result.result.Succeeded) return Results.ValidationProblem(result.result.Errors.ConvertToProblemDetails());
        return Results.Created($"/employees/{result.id}", result.id);
    }
}
