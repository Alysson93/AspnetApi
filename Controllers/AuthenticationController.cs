using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using AspnetApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _manager;

    public AuthenticationController(UserManager<IdentityUser> manager)
    {
        _manager = manager;
    }

    [HttpPost("login")]
    public IResult Login([FromBody] LoginDTO request)
    {
        var user = _manager.FindByEmailAsync(request.Email).Result;
        if (user == null || !_manager.CheckPasswordAsync(user, request.Password).Result)
            return Results.BadRequest();
        var key = Encoding.ASCII.GetBytes("Abcdefghijkl123!@4567890mnopqrstuv");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Email, request.Email)
            ]),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature
            ),
            Audience = "AspNetApi",
            Issuer = "Issuer"
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new {token = tokenHandler.WriteToken(token)});
    }
}
