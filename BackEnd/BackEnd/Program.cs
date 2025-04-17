using System;
using System.Text;
using System.Text.Json;
using BackEnd.Data;
using BackEnd.Entities;
using BackEnd.UseCases.Auth;
using BackEnd.UseCases.HotelRooms;
using BackEnd.UseCases.Bookings;
using BackEnd.UseCases.Payments;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configuration
var config            = builder.Configuration;
var jwtKey            = config["Jwt:Key"];
var jwtIssuer         = config["Jwt:Issuer"];
var jwtAudience       = config["Jwt:Audience"];
var connectionString  = config.GetConnectionString("DefaultConnection");
const string corsPolicyName = "_allowFrontend";

// 🔐 Validate required settings
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new InvalidOperationException("JWT Key must be at least 32 characters long");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Missing connection string.");

// 🔹 Services
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(connectionString)
);

builder.Services
    .AddIdentityCore<Admin>(opts => {})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 🔐 JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unauthorized" }));
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Forbidden" }));
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtIssuer,
            ValidAudience            = jwtAudience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// 🔹 Dependency Injection
builder.Services.AddScoped<RegisterUser>();
builder.Services.AddScoped<LoginUser>();
builder.Services.AddScoped<GenerateJwtToken>();
builder.Services.AddScoped<GetHotelRooms>();
builder.Services.AddScoped<GetHotelRoom>();
builder.Services.AddScoped<CreateHotelRoom>();
builder.Services.AddScoped<UpdateHotelRoom>();
builder.Services.AddScoped<DeleteHotelRoom>();
builder.Services.AddScoped<CreateBooking>();
builder.Services.AddScoped<GetBooking>();
builder.Services.AddScoped<GetMyBookings>();
builder.Services.AddScoped<DeleteBooking>();
builder.Services.AddScoped<CreatePayment>();

// 🌐 CORS
builder.Services.AddCors(opts =>
{
    opts.AddPolicy(corsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// 🔎 Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter your JWT token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.WriteIndented      = true;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = 500;
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var json  = JsonSerializer.Serialize(new
        {
            error  = "An unexpected error occurred.",
            detail = error?.Message
        });
        await context.Response.WriteAsync(json);
    });
});

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await AppDbContext.SeedAdmin(scope.ServiceProvider); // ✅ Seed admin user at runtime
}

app.Run();
