using E_COmmerce.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using E_Commerce.Persistence.DBInitializers;
using E_Commerce.Persistence.Repositories;
using StackExchange.Redis;
using E_Commerce.Service.Abstraction;
using E_Commerce.Persistence.Services;
using E_Commerce.Persistence.AuthContext;
using E_Commerce.Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;


namespace E_Commerce.Persistence.DependencyInjection;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services
        , IConfiguration configuration )
    {
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("AuthConnection"));
        });
        services.AddScoped<ICashService, CashService>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddSingleton<IConnectionMultiplexer>(cfg =>
        {
            return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!);
        });
        services.AddDbContext<StoreDbContext>(options =>
        {
            var connection = configuration.GetConnectionString("SQLConnection");
            options.UseSqlServer(connection);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDInitializer, DBInitializer>();
        ConfigureIdentity(services, configuration);
        return services;
    }
    private static void ConfigureIdentity(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<ApplicationUser>(c =>
        {
            c.Password.RequireDigit = false;
            c.Password.RequireLowercase = false;
            c.Password.RequireUppercase = false;
            c.Password.RequireNonAlphanumeric = false;
            c.Password.RequiredLength = 4;
        }).AddRoles<IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();
    }
}
