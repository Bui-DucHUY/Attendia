using Attendia.Data;
using Attendia.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Dapper Context as a Singleton (it only holds configuration)
builder.Services.AddSingleton<DapperContext>();

// Register Repositories
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
// builder.Services.AddScoped<IInstructorRepository, InstructorRepository>(); // Add others as you build them
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
// 1. Register the Instructor Repository
builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();

builder.Services.AddScoped<IClassroomRepository, ClassroomRepository>();
// 2. Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();