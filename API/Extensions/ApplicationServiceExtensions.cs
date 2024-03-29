using API.Providers;
using Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Providers.API;
using Providers.Image;
using Providers.Mail;
using Providers.Security;
using Services.Core;
using Services.Interfaces;
using Services.User;
using Services.User.Utils;
using Services.User.Utils.Interfaces;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GoBarber API",
                Version = "v1"
            });
        });
        services.AddDbContext<DataContext>(options =>
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string connStr;

            // Depending on if in development or production, use either Heroku-provided
            // connection string, or development connection string from env var.
            if (env == "Development") connStr = config.GetConnectionString("DefaultConnection");
            else
            {
                // Use connection string provided at runtime by Heroku.
                var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                // Parse connection URL to connection string for Npgsql
                connUrl = connUrl?.Replace("postgres://", string.Empty);
                var pgUserPass = connUrl?.Split("@")[0];
                var pgHostPortDb = connUrl?.Split("@")[1];
                var pgHostPort = pgHostPortDb?.Split("/")[0];
                var pgDb = pgHostPortDb?.Split("/")[1];
                var pgUser = pgUserPass?.Split(":")[0];
                var pgPass = pgUserPass?.Split(":")[1];
                var pgHost = pgHostPort?.Split(":")[0];
                var pgPort = pgHostPort?.Split(":")[1];

                connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
            }

            // Whether the connection string came from the local development configuration file
            // or from the environment variable from Heroku, use it to set up your DbContext.
            options.UseNpgsql(connStr);
        });
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins("https://localhost:3000");
            });
        });
        services
            .AddFluentEmail(config["Mail:Email"])
            .AddRazorRenderer()
            .AddSmtpSender(
                config.GetValue<string>("Mail:Host"),
                config.GetValue<int>("Mail:Port"),
                config.GetValue<string>("Mail:User"),
                config.GetValue<string>("Mail:Password")
            );

        services.AddMediatR(typeof(Create.Handler).Assembly);
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);

        services.AddScoped<IApiAccessor, ApiAccessor>();
        services.AddScoped<IImageAccessor, ImageAccessor>();
        services.AddScoped<ITokenAccessor, TokenAccessor>();
        services.AddScoped<IMailAccessor, MailAccessor>();
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IUserRefreshToken, UserRefreshToken>();

        return services;
    }
}
