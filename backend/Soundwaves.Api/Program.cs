using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("SoundwavesDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'SoundwavesDatabase' was not found.");

builder.Services.AddDbContext<SoundwavesDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<IMusicCatalogService, MusicCatalogService>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
