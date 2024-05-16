using System.Security.Claims;
using AspnetApi.Data;
using AspnetApi.Dtos;
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

    [HttpPost]
    public IResult Post(EmployeeRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };
        var result = _manager.CreateAsync(user, request.Password).Result;
        if (!result.Succeeded) return Results.BadRequest(result.Errors.First());
        var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode", request.Code),
            new Claim("EmployeeName", request.Name)
        };
        var claimResult = _manager.AddClaimsAsync(user, userClaims).Result;
        if (!claimResult.Succeeded) return Results.BadRequest(claimResult.Errors.First());
        return Results.Created($"/employees/{user.Id}", user.Id);
    }
}
