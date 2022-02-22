using System.Security.Claims;
using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Providers.Security;

public class IsBarberRequirement : IAuthorizationRequirement
{
}

public class IsBarberRequirementHandler : AuthorizationHandler<IsBarberRequirement>
{
    private readonly DataContext _context;
    public IsBarberRequirementHandler(DataContext context)
    {
        this._context = context;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsBarberRequirement requirement)
    {
        var userEmail = context.User.FindFirstValue(ClaimTypes.Email);

        if (userEmail == null) return Task.CompletedTask;

        var user = this._context.Users.AsNoTracking().SingleOrDefault(x => x.Email == userEmail);

        if (user == null) return Task.CompletedTask;

        if (user.IsBarber) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
