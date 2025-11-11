using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Api.Middlewares;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Services;
using Vogel.Rentals.Application.Validation;
using Vogel.Rentals.Infrastructure.Contexts;
using Vogel.Rentals.Infrastructure.Local;
using Vogel.Rentals.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();

builder.Services.AddScoped<IMotorcycleService, MotorcycleService>();
builder.Services.AddScoped<ICourierService, CourierService>();
builder.Services.AddScoped<IRentalService, RentalService>();
// TODO: Temp, change to S3
builder.Services.AddSingleton<IStorageService, LocalStorageService>();

builder.Services.AddScoped<IMotorcycleValidator, MotorcycleValidator>();
builder.Services.AddScoped<ICourierValidator, CourierValidator>();
builder.Services.AddScoped<IRentalValidator, RentalValidator>();

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