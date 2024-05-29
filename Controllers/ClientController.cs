using System.Security.Claims;
using AspnetApi.Dtos;
using AspnetApi.Services;
using AspnetApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/clients")]
public class ClientController : ControllerBase
{
    private readonly UserService _service;

    public ClientController(UserService service)
    {
        _service = service;
    }

    [HttpPost] [AllowAnonymous]
    public async Task<IResult> Post(ClientRequest request)
    {
        var userClaims = new List<Claim>
        {
            new Claim("Cpf", request.Cpf),
            new Claim("Name", request.Name)
        };
        (IdentityResult result, string id) result = await _service.Create(request.Email, request.Password, userClaims);
        if (!result.result.Succeeded) return Results.ValidationProblem(result.result.Errors.ConvertToProblemDetails());
        return Results.Created($"/clients/{result.id}", result.id);
    }
}
