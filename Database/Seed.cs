using Microsoft.AspNetCore.Identity;
using Models;

namespace Database;

public static class Seed
{
    public static async Task SeedData(UserManager<AppUser> userManager)
    {
        if (!userManager.Users.Any())
        {
            var users = new List<AppUser>
            {
                new AppUser
                {
                    DisplayName = "Bob",
                    UserName = "bob",
                    Email = "bob@test.com",
                },
                new AppUser
                {
                    DisplayName = "Tom",
                    UserName = "tom",
                    Email = "tom@test.com"
                },
                new AppUser
                {
                    DisplayName = "Jane",
                    UserName = "jane",
                    Email = "jane@test.com"
                },
                new AppUser
                {
                    DisplayName = "Bernardo Generoso",
                    UserName = "bernardogeneroso",
                    Email = "bernardogeneroso@test.com"
                },
                new AppUser
                {
                    DisplayName = "John",
                    UserName = "john",
                    Email = "john@test.com",
                    IsBarber = true
                },
                new AppUser
                {
                    DisplayName = "Agostinho",
                    UserName = "agostinho",
                    Email = "agostinho@test.com",
                    IsBarber = true
                }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
