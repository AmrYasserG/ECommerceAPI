using ECommerceAPI.BLL.Managers;
using ECommerceAPI.BLL.Mapping;
using ECommerceAPI.BLL.Services;
using ECommerceAPI.BLL.Validators;
using ECommerceAPI.Common;
using ECommerceAPI.DAL.Context;
using ECommerceAPI.DAL.Identity;
using ECommerceAPI.DAL.Repositories;
using ECommerceAPI.DAL.UnitOfWork;
using ECommerceAPI.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace ECommerceAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Controllers ──────────────────────────────────────────────────────────
            builder.Services.AddControllers();

            // ── CORS ─────────────────────────────────────────────────────────────────
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            // ── Identity ─────────────────────────────────────────────────────────────
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength      = 8;
                options.Password.RequireDigit        = true;
                options.Password.RequireUppercase    = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail      = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ── JWT Authentication ────────────────────────────────────────────────────
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = builder.Configuration["Jwt:Issuer"],
                    ValidAudience            = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            // ── Authorization Policies ────────────────────────────────────────────────
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",         policy => policy.RequireRole("Admin"));
                options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
            });

            // ── AutoMapper ────────────────────────────────────────────────────────────
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // ── FluentValidation ──────────────────────────────────────────────────────
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return new BadRequestObjectResult(
                        ApiResponse<object>.FailResult("Validation failed.", errors));
                };
            });

            // ── DbContext ─────────────────────────────────────────────────────────────
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ── Repositories ──────────────────────────────────────────────────────────
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IProductRepository,  ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICartRepository,     CartRepository>();
            builder.Services.AddScoped<IOrderRepository,    OrderRepository>();

            // ── Unit of Work ──────────────────────────────────────────────────────────
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Managers ─────────────────────────────────────────────────────────────
            builder.Services.AddScoped<IProductManager,  ProductManager>();
            builder.Services.AddScoped<ICategoryManager, CategoryManager>();
            builder.Services.AddScoped<ICartManager,     CartManager>();
            builder.Services.AddScoped<IOrderManager,    OrderManager>();
            builder.Services.AddScoped<IAuthManager,     AuthManager>();

            // ── Services ──────────────────────────────────────────────────────────────
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IImageService, ImageService>();

            // ── OpenAPI ───────────────────────────────────────────────────────────────
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // ── Seed Roles & Admin User ───────────────────────────────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                foreach (var role in new[] { "Admin", "User" })
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                const string adminEmail = "admin@ecommerce.com";
                if (await userManager.FindByEmailAsync(adminEmail) is null)
                {
                    var admin = new AppUser
                    {
                        FullName = "Admin",
                        Email    = adminEmail,
                        UserName = adminEmail
                    };
                    await userManager.CreateAsync(admin, "Admin@123456");
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // ── Middleware Pipeline ───────────────────────────────────────────────────
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
