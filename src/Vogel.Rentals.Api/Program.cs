using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Api.Middlewares;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Infrastructure.Contexts;
using Vogel.Rentals.Infrastructure.InMemory;
using Vogel.Rentals.Infrastructure.Local;
using Vogel.Rentals.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
// TODO: Temp, change to DB
builder.Services.AddSingleton<ICourierRepository, InMemoryCourierRepository>();
builder.Services.AddSingleton<IRentalRepository, InMemoryRentalRepository>();
// TODO: Temp, change to S3
builder.Services.AddSingleton<IStorageService, LocalStorageService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<RentalsDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/hc", () => Results.Ok(new { status = "ok" }));

app.Run();