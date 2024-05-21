using System.Security.Claims;
using AspnetApi.Data;
using AspnetApi.Dtos;
using AspnetApi.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/employees")]
public class EmployeeController : ControllerBase
{
    private readonly UserManager<IdentityUser> _manager;
    private readonly AppDbContext _context;

    public EmployeeController(UserManager<IdentityUser> manager, AppDbContext context)
    {
        _manager = manager;
        _context = context;
    }

    [HttpGet]
    public IResult Get([FromQuery] int skip = 0, [FromQuery] int take = 5)
    {
        var employees = (
            from user in _context.Users
            join claim in _context.UserClaims
            on user.Id equals claim.UserId
            where claim.ClaimType == "EmployeeName"
            orderby claim.ClaimValue
            select new EmployeeResponse(user.Email, claim.ClaimValue)
        ).Skip(skip).Take(take);
        return Results.Ok(employees);
    }

    [HttpPost]
    public IResult Post(EmployeeRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };
        var result = _manager.CreateAsync(user, request.Password).Result;
        if (!result.Succeeded) return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode", request.Code),
            new Claim("EmployeeName", request.Name)
        };
        var claimResult = _manager.AddClaimsAsync(user, userClaims).Result;
        if (!claimResult.Succeeded) return Results.ValidationProblem(claimResult.Errors.ConvertToProblemDetails());
        return Results.Created($"/employees/{user.Id}", user.Id);
    }
}
