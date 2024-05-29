using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AspnetApi.Services;

public class UserService
{
    private readonly UserManager<IdentityUser> _manager;

    public UserService(UserManager<IdentityUser> manager)
    {
        _manager = manager;
    }

    public async Task<(IdentityResult, string)> Create(string email, string password, List<Claim> claims)
    {
        var user = new IdentityUser{UserName = email, Email = email};
        var result = await _manager.CreateAsync(user, password);
        if (!result.Succeeded) return (result, string.Empty);
        return (await _manager.AddClaimsAsync(user, claims), user.Id);
    }
}