using Microsoft.EntityFrameworkCore;
using GradifyApi.Data;
using GradifyApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using NSwag.AspNetCore.Middlewares;


using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();//Service for controllers

builder.Services.AddScoped<JwtTokenService>();//Service for JwtTokenService class

//Service for database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DatabaseConnection")
    )
);

//Variables for backend api Jwt token information
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

//Adding JWT token authentication service 
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,          
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),
        };
    });

//Adding JWT token authorization service 
builder.Services.AddAuthorization();

//Smtp settings configuration as a service for dependency injection
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

//EmailService class as a object added to the service scope for dependency injectio
builder.Services.AddScoped<EmailService>();

//Documentation for swagger 
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Gradify API";

    // Add "Bearer" security definition
    config.AddSecurity("Bearer", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste your JWT access token here. Example: eyJhbGciOi..."
    });

    // Apply it to endpoints 
    config.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
});

//Build application
var app = builder.Build();


//enable services before running application
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();       
    app.UseSwaggerUi(c =>
    {
        c.Path = "/swagger"; 

    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

