using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskBridge.Api.Common.Persistence;
using TaskBridge.Api.Projects;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddDbContext<TaskBridgeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TaskBridge")));
builder.Services.AddScoped<IProjectService, ProjectService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskBridgeDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapControllers();
app.Run();

public partial class Program;