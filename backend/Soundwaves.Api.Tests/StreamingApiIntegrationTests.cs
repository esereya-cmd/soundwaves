using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soundwaves.Api.Data;
using Soundwaves.Api.Models;

namespace Soundwaves.Api.Tests;

public class StreamingApiIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _temporaryDirectory;

    private readonly byte[] _audioBytes =
        Enumerable.Range(0, 2048)
            .Select(i => (byte)(i % 256))
            .ToArray();

    public StreamingApiIntegrationTests()
    {
        _temporaryDirectory = Path.Combine(
            Path.GetTempPath(),
            "SoundwavesStreamingTests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_temporaryDirectory);

        // Create a test-only audio fixture.
        File.WriteAllBytes(
            Path.Combine(_temporaryDirectory, "valid.mp3"),
            _audioBytes);

        var databaseName =
            "SoundwavesStreaming_" + Guid.NewGuid().ToString("N");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["MediaStorage:RootDirectory"] =
                                _temporaryDirectory
                        });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<
                        DbContextOptions<SoundwavesDbContext>>();

                    services.AddDbContext<SoundwavesDbContext>(
                        options => options.UseInMemoryDatabase(
                            databaseName));
                });
            });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<SoundwavesDbContext>();

        db.Tracks.AddRange(
            new Track
            {
                Id = 9101,
                Title = "Streaming Test Track",
                FilePath = "valid.mp3",
                MimeType = "audio/mpeg",
                FileSizeBytes = (ulong)_audioBytes.Length,
                CreatedAt = DateTime.UtcNow
            },
            new Track
            {
                Id = 9102,
                Title = "Missing Audio Test",
                FilePath = "missing.mp3",
                MimeType = "audio/mpeg",
                CreatedAt = DateTime.UtcNow
            },
            new Track
            {
                Id = 9103,
                Title = "Traversal Security Test",
                FilePath = "../outside.mp3",
                MimeType = "audio/mpeg",
                CreatedAt = DateTime.UtcNow
            });

        db.SaveChanges();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();

        if (Directory.Exists(_temporaryDirectory))
        {
            Directory.Delete(
                _temporaryDirectory,
                recursive: true);
        }
    }

    [Fact]
    public async Task TestFixture_ResolvesValidAudioFile()
    {
        using var scope = _factory.Services.CreateScope();

        var lookupService = scope.ServiceProvider
            .GetRequiredService<Soundwaves.Api.Services.IAudioFileLookupService>();

        var result = await lookupService.GetAudioFilePathAsync(9101);

        Assert.Equal(
            Path.Combine(_temporaryDirectory, "valid.mp3"),
            result);
    }}

