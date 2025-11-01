using E_Commerce.Domain.Contracts;
using E_Commerce.Persistence.DependencyInjection;
using E_Commerce.Service.DependencyInjections;
using ECommerce.Web.Handlers;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace ECommerce.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddApplicationServices().AddPersistenceServices(builder.Configuration).AddInfrastructureServices(builder.Configuration);
            builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection(JWTOptions.SectionName));
            builder.Services.AddApplicationServices();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            e => e.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    var problem = new ProblemDetails
                    {
                        Title = "One or more validation errors occurred.",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "See the errors property for details.",
                        Extensions =
                        {
                            { "errors", errors }
                        }
                    };
                    return new BadRequestObjectResult("");
                    
                };
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwt = builder.Configuration.GetSection(JWTOptions.SectionName).Get<JWTOptions>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.key)),
                    ClockSkew = TimeSpan.Zero
                };
            });
            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDInitializer>();
            await initializer.InitializeAsync();
            await initializer.InitializeAuthDbAsync();


            app.UseExceptionHandler();

            ///app.Use(async (context, next) =>
            ///{
            ///    try
            ///    {
            ///        await next.Invoke(context);
            ///    }
            ///    catch (Exception ex)
            ///    {
            ///        Console.WriteLine(ex.Message);
            ///        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ///        await context.Response.WriteAsJsonAsync(new
            ///        {
            ///            StatusCode = StatusCodes.Status500InternalServerError,
            ///            Message = ex.Message
            ///        });
            ///    }
            ///});
            /// Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
