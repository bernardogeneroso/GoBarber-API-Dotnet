using System.Security.Claims;
using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Providers.Security;

public class IsBarberRequirement : IAuthorizationRequirement
{
}

public class IsBarberRequirementHandler : AuthorizationHandler<IsBarberRequirement>
{
    private readonly DataContext _dataContext;
    public IsBarberRequirementHandler(DataContext dataContext)
    {
        this._dataContext = dataContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsBarberRequirement requirement)
    {
        var userEmail = context.User.FindFirstValue(ClaimTypes.Email);

        if (userEmail == null) return Task.CompletedTask;

        var user = this._dataContext.Users.AsNoTracking().SingleOrDefault(x => x.Email == userEmail);

        if (user == null) return Task.CompletedTask;

        if (user.IsBarber)
        {
            context.User.AddIdentity(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }));
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
