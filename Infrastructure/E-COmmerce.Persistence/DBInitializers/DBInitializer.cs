using E_Commerce.Domain.Entities.Auth;
using E_Commerce.Persistence.AuthContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace E_Commerce.Persistence.DBInitializers;

internal class DBInitializer(StoreDbContext dbContext, AuthDbContext authDbContext, RoleManager<IdentityRole> role, 
    UserManager<ApplicationUser> user, ILogger<DBInitializer> logger) 
    : IDInitializer
{
    public async Task InitializeAsync()
    {
        try
        {
            if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
                await dbContext.Database.MigrateAsync();
            if (!await dbContext.Products.AnyAsync())
            {
                var BrandData = await File.ReadAllTextAsync(
                    "@Infrastructure/E-COmmerce.Persistence/DBInitializers/SeedData/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandData);
                if (brands != null && brands.Any())
                {
                    dbContext.ProductBrands.AddRange(brands);
                }
                await dbContext.SaveChangesAsync();
            }
            if (!dbContext.ProductsTypes.Any())
            {
                var TypeData = await File.ReadAllTextAsync(
                    "@Infrastructure/E-COmmerce.Persistence/DBInitializers/SeedData/types.json");
                var types = JsonSerializer.Deserialize<List<ProductsType>>(TypeData);
                if (types != null && types.Any())
                {
                    dbContext.ProductsTypes.AddRange(types);
                }
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during database initialization: {ex.Message}");
        }
    }

    public async Task InitializeAuthDbAsync()
    {
        await authDbContext.Database.MigrateAsync();
        if (!role.Roles.Any())
        {
            await role.CreateAsync(new IdentityRole("Admin"));
            await role.CreateAsync(new IdentityRole("User"));
        }
        if (!role.Roles.Any())
        {
            var adminUser = new ApplicationUser
            {
                DisplayName = "Admin User",
                Email = "admin@gmail.com",
                UserName = "admin",
                PhoneNumber = "1234567890",
            };
            var User = new ApplicationUser
            {
                DisplayName = "User",
                Email = "user@gmail.com",
                UserName = "user",
                PhoneNumber = "1234567890",
            };
            await user.CreateAsync(adminUser, "Passw0rd");
            await user.CreateAsync(User, "Passw0rd");
            await user.AddToRoleAsync(adminUser, "Passw0rd");
            await user.AddToRoleAsync(User, "Passw0rd");
        }
    }
}
