using Dairy.Context;
using Dairy.DTO;
using Dairy.DMO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// DB Config
var connectionString = builder.Configuration["MongoDb:ConnectionString"] ?? "mongodb://localhost:27018";
var databaseName = builder.Configuration["MongoDb:DatabaseName"] ?? "DairyDB";

builder.Services.AddSingleton(new DairyRepository(connectionString, databaseName));
builder.Services.AddSingleton(new UserRepository(connectionString, databaseName));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsAVerySecretKeyForJwtAuthenticationWhichNeedsToBeLongEnough";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Dairy.ServiceHub",
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Login Endpoint
app.MapPost("/api/auth/login", async (LoginRequest request, UserRepository userRepo) =>
{
    var user = await userRepo.GetUserAsync(request.Username, request.Password);
    if (user == null) return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = builder.Configuration["Jwt:Issuer"] ?? "Dairy.ServiceHub",
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new AuthResponse { Token = tokenHandler.WriteToken(token), Role = user.Role });
});

// Get Products (Accessible by both Admin and User, but returns different fields)
app.MapGet("/api/dairy/products", async (DairyRepository repository, ClaimsPrincipal user) =>
{
    var products = await repository.GetAllProductsAsync();
    bool isAdmin = user.IsInRole("Admin");

    if (isAdmin)
    {
        var response = products.Select(p => new DairyProductAdminResponse
        {
            ProductId = p.Id.ToString(),
            Name = p.Name ?? "Unknown Product",
            FatContent = p.FatContentPercentage,
            TemperatureRequired = p.StorageTemperatureRange,
            StockQuantity = p.StockQuantity,
            IsFresh = (DateTime.UtcNow - p.PasteurizationDate).TotalDays <= 14
        }).ToList();
        return Results.Ok(response);
    }
    else
    {
        var response = products.Select(p => new DairyProductResponse
        {
            ProductId = p.Id.ToString(),
            Name = p.Name ?? "Unknown Product",
            FatContent = p.FatContentPercentage,
            IsFresh = (DateTime.UtcNow - p.PasteurizationDate).TotalDays <= 14
        }).ToList();
        return Results.Ok(response);
    }
}).RequireAuthorization();

// Add Product (Admin Only)
app.MapPost("/api/dairy/products", async (DairyProductRequest request, DairyRepository repository) =>
{
    var product = new DairyProduct
    {
        Name = request.Name,
        FatContentPercentage = request.FatContentPercentage,
        StorageTemperatureRange = request.StorageTemperatureRange,
        StockQuantity = request.StockQuantity,
        PasteurizationDate = DateTime.UtcNow
    };
    await repository.AddProductAsync(product);
    return Results.Ok(product);
}).RequireAuthorization("AdminOnly");

// Update Product (Admin Only)
app.MapPut("/api/dairy/products/{id}", async (string id, DairyProductRequest request, DairyRepository repository) =>
{
    var existing = await repository.GetProductAsync(id);
    if (existing == null) return Results.NotFound();

    existing.Name = request.Name;
    existing.FatContentPercentage = request.FatContentPercentage;
    existing.StorageTemperatureRange = request.StorageTemperatureRange;
    existing.StockQuantity = request.StockQuantity;

    await repository.UpdateProductAsync(id, existing);
    return Results.Ok();
}).RequireAuthorization("AdminOnly");

// Delete Product (Admin Only)
app.MapDelete("/api/dairy/products/{id}", async (string id, DairyRepository repository) =>
{
    var existing = await repository.GetProductAsync(id);
    if (existing == null) return Results.NotFound();

    await repository.DeleteProductAsync(id);
    return Results.Ok();
}).RequireAuthorization("AdminOnly");

app.Run();
