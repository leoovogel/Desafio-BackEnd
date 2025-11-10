using Mottu.Rentals.Application.Abstractions;
using Mottu.Rentals.Infrastructure.InMemory;
using Mottu.Rentals.Infrastructure.Local;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// TODO: Temp, change to DB
builder.Services.AddSingleton<IMotorcycleRepository, InMemoryMotorcycleRepository>();
builder.Services.AddSingleton<ICourierRepository, InMemoryCourierRepository>();
// TODO: Temp, change to S3
builder.Services.AddSingleton<IStorageService, LocalStorageService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/hc", () => Results.Ok(new { status = "ok" }));

app.Run();