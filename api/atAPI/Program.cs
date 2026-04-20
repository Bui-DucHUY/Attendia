using Attendia.Data;
using Attendia.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Dapper Context as a Singleton (it only holds configuration)
builder.Services.AddSingleton<DapperContext>();

// Register Repositories
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
// builder.Services.AddScoped<IInstructorRepository, InstructorRepository>(); // Add others as you build them

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();