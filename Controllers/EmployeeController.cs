using System.Security.Claims;
using AspnetApi.Data;
using AspnetApi.Dtos;
using AspnetApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Expressions;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/employees")]
public class EmployeeController : ControllerBase
{
    private readonly UserManager<IdentityUser> _manager;
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _http;

    public EmployeeController(
        UserManager<IdentityUser> manager, 
        AppDbContext context,
        IHttpContextAccessor http)
    {
        _manager = manager;
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
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };
        var result = await _manager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        var userClaims = new List<Claim>
        {
            new Claim("Code", request.Code),
            new Claim("Name", request.Name),
            new Claim("CreatedBy", userId)
        };
        var claimResult = await _manager.AddClaimsAsync(user, userClaims);
        if (!claimResult.Succeeded) return Results.ValidationProblem(claimResult.Errors.ConvertToProblemDetails());
        return Results.Created($"/employees/{user.Id}", user.Id);
    }
}
