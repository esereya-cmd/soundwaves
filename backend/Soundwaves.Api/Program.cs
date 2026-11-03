
using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<SoundwavesDbContext>(options =>
        options.UseInMemoryDatabase("SoundwavesIntegrationTests"));
}
else
{
    var connectionString = builder.Configuration
        .GetConnectionString("SoundwavesDatabase")
        ?? throw new InvalidOperationException(
            "Connection string 'SoundwavesDatabase' was not found.");

    builder.Services.AddDbContext<SoundwavesDbContext>(options =>
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)));
}

// Register backend services.
builder.Services.AddScoped<IMusicCatalogService, MusicCatalogService>();

// SCRUM-131: Secure audio file lookup service.
builder.Services.AddScoped<IAudioFileLookupService, AudioFileLookupService>();

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

public partial class Program { }
