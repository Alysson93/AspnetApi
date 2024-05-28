using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspnetApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspnetApi.Controllers;

[ApiController]
[Route("/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<IdentityUser> _manager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AuthenticationController(
        UserManager<IdentityUser> manager, 
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _manager = manager;
        _configuration = configuration;
        _webHostEnvironment = environment;
    }

    [HttpPost("login")] [AllowAnonymous]
    public IResult Login([FromBody] LoginDTO request)
    {
        var user = _manager.FindByEmailAsync(request.Email).Result;
        Console.WriteLine("O user é " + user);
        if (user == null || !_manager.CheckPasswordAsync(user, request.Password).Result)
            return Results.BadRequest();
        var claims = _manager.GetClaimsAsync(user).Result;
        var subject = new ClaimsIdentity([
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        ]);
        subject.AddClaims(claims);
        var key = Encoding.ASCII.GetBytes(_configuration["JwtBearerTokenSettings:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature
            ),
            Audience = _configuration["JwtBearerTokenSettings:Audience"],
            Issuer = _configuration["JwtBearerTokenSettings:Issuer"],
            Expires = _webHostEnvironment.IsDevelopment() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(60)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new {token = tokenHandler.WriteToken(token)});
    }
}
