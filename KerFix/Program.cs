
using KerFix.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<KerFixDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("KerFixConnection")
    )
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<KerFixDbContext>();

    if (db.Database.CanConnect())
    {
        Console.WriteLine("KerFix: PostgreSQL conectado correctamente");
    }
    else
    {
        Console.WriteLine("KerFix: No se pudo conectar a PostgreSQL");
    }
}

app.Run();
