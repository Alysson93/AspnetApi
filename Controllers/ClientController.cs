using System.Security.Claims;
using AspnetApi.Dtos;
using AspnetApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/clients")]
public class ClientController : ControllerBase
{
    private readonly UserManager<IdentityUser> _manager;

    public ClientController(UserManager<IdentityUser> manager)
    {
        _manager = manager;
    }

    [HttpPost] [AllowAnonymous]
    public async Task<IResult> Post(ClientRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };
        var result = await _manager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return Results.ValidationProblem(result.Errors.ConvertToProblemDetails());
        var userClaims = new List<Claim>
        {
            new Claim("Cpf", request.Cpf),
            new Claim("Name", request.Name)
        };
        var claimResult = await _manager.AddClaimsAsync(user, userClaims);
        if (!claimResult.Succeeded) return Results.ValidationProblem(claimResult.Errors.ConvertToProblemDetails());
        return Results.Created($"/clients/{user.Id}", user.Id);
    }
}
