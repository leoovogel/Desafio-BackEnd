using Microsoft.EntityFrameworkCore;
using Vogel.Rentals.Api.Middlewares;
using Vogel.Rentals.Application.Abstractions;
using Vogel.Rentals.Application.Services;
using Vogel.Rentals.Application.Validation;
using Vogel.Rentals.Infrastructure.Contexts;
using Vogel.Rentals.Infrastructure.Local;
using Vogel.Rentals.Infrastructure.Messaging;
using Vogel.Rentals.Infrastructure.Repositories;
using Vogel.Rentals.Infrastructure.Storage;

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

var awsKey = builder.Configuration["AWS_ACCESS_KEY_ID"];
var awsSecret = builder.Configuration["AWS_SECRET_ACCESS_KEY"];
var bucket = builder.Configuration["S3Storage:BucketName"];

if (!string.IsNullOrEmpty(awsKey) && !string.IsNullOrEmpty(awsSecret) && !string.IsNullOrEmpty(bucket))
{
    builder.Services.Configure<S3Options>(builder.Configuration.GetSection("S3Storage"));
    builder.Services.AddSingleton<IStorageService, S3StorageService>();
    Console.WriteLine("Using Amazon S3 storage service");
}
else
{
    builder.Services.AddSingleton<IStorageService, LocalStorageService>();
    Console.WriteLine("Using local storage service");
}

builder.Services.Configure<RabbitmQOptions>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<IMotorcycleEventPublisher, RabbitMqMotorcycleEventPublisher>();
builder.Services.AddHostedService<MotorcycleCreatedConsumer>();

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