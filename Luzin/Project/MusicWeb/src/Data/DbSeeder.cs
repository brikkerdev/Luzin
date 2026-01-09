using Microsoft.EntityFrameworkCore;
using MusicWeb.Services.Auth;
using MusicWeb.src.Data;
using MusicWeb.src.Models.Entities;
using MusicWeb.src.Models.Enums;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(ApiDbContext db, IPasswordHasher hasher)
    {
        if (await db.Users.AnyAsync(u => u.Username == "admin"))
            return;

        var admin = new User
        {
            Username = "admin",
            PasswordHash = hasher.Hash("admin"),
            Role = Role.Admin,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}