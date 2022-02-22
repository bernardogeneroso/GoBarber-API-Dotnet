using System.Text;
using API.Providers;
using Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Models;
using Providers.Security;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<AppUser>(opt =>
                {
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.User.RequireUniqueEmail = true;
                    opt.SignIn.RequireConfirmedEmail = true;
                    // opt.SignIn.RequireConfirmedEmail = true;
                }).AddEntityFrameworkStores<DataContext>()
                  .AddSignInManager<SignInManager<AppUser>>()
                  .AddDefaultTokenProviders();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("IsBarber", policy =>
            {
                policy.Requirements.Add(new IsBarberRequirement());
            });
        });
        services.AddTransient<IAuthorizationHandler, IsBarberRequirementHandler>();

        return services;
    }
}
