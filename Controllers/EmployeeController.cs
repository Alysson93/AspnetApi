using System.Security.Claims;
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

    public EmployeeController(UserManager<IdentityUser> manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public IResult Get([FromQuery] int skip = 0, [FromQuery] int take = 5)
    {
        var users = _manager.Users.Skip(skip).Take(take).ToList();
        var employees = new List<EmployeeResponse>();
        foreach(var item in users)
        {
            var claims = _manager.GetClaimsAsync(item).Result;
            var claimName = claims.FirstOrDefault(c => c.Type == "EmployeeName");
            var username = claimName != null ? claimName.Value : string.Empty;
            employees.Add(new EmployeeResponse(item.Email, username));
        }
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
