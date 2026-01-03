using BuildsOfTitansNet.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using System;
using BuildsOfTitansNet.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);
var AllowedOriginsName = "corsOriginsBuildsOfTitansNet";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowedOriginsName,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();

                      });
});
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "default_secret_key_please_change")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("DB_PORT")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USERNAME")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}




var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("Health")
    .WithOpenApi()
    .Produces(200);

app.UseCors(AllowedOriginsName);
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
