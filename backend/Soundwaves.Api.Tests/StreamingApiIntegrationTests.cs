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
    }
    [Theory]
    [InlineData(9102UL)]
    [InlineData(9103UL)]
    public async Task TestFixture_RejectsUnavailableOrUnsafeAudio(ulong trackId)
    {
        using var scope = _factory.Services.CreateScope();

        var lookupService = scope.ServiceProvider
            .GetRequiredService<Soundwaves.Api.Services.IAudioFileLookupService>();

        var result = await lookupService.GetAudioFilePathAsync(trackId);

        Assert.Null(result);
    }

    [Fact]
    public async Task StreamTrack_ReturnsPartialContent_ForValidRange()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, "/api/tracks/9101/stream");

        request.Headers.Range =
            new System.Net.Http.Headers.RangeHeaderValue(0, 1023);

        using var response = await _client.SendAsync(request);

        Assert.Equal(
            System.Net.HttpStatusCode.PartialContent,
            response.StatusCode);

        Assert.Equal(
            "audio/mpeg",
            response.Content.Headers.ContentType?.MediaType);

        Assert.Equal(1024, response.Content.Headers.ContentLength);

        Assert.Equal(
            0,
            response.Content.Headers.ContentRange?.From);

        Assert.Equal(
            1023,
            response.Content.Headers.ContentRange?.To);

        Assert.Equal(
            _audioBytes.Length,
            response.Content.Headers.ContentRange?.Length);

        var bytes = await response.Content.ReadAsByteArrayAsync();

        Assert.Equal(_audioBytes.Take(1024).ToArray(), bytes);
    }

    [Fact]
    public async Task StreamTrack_ReturnsRangeNotSatisfiable_ForInvalidRange()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, "/api/tracks/9101/stream");

        request.Headers.Range =
            new System.Net.Http.Headers.RangeHeaderValue(999999, 1000000);

        using var response = await _client.SendAsync(request);

        Assert.Equal(
            System.Net.HttpStatusCode.RequestedRangeNotSatisfiable,
            response.StatusCode);

        Assert.Equal(
            _audioBytes.Length,
            response.Content.Headers.ContentRange?.Length);

        Assert.Null(response.Content.Headers.ContentRange?.From);
        Assert.Null(response.Content.Headers.ContentRange?.To);
    }

    [Fact]
    public async Task StreamTrack_ReturnsRemainingBytes_ForOpenEndedRange()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, "/api/tracks/9101/stream");

        request.Headers.Range =
            new System.Net.Http.Headers.RangeHeaderValue(1024, null);

        using var response = await _client.SendAsync(request);

        Assert.Equal(
            System.Net.HttpStatusCode.PartialContent,
            response.StatusCode);

        Assert.Equal(
            _audioBytes.Length - 1024,
            response.Content.Headers.ContentLength);

        var bytes = await response.Content.ReadAsByteArrayAsync();

        Assert.Equal(_audioBytes.Skip(1024).ToArray(), bytes);
    }

    [Fact]
    public async Task StreamTrack_ReturnsLastBytes_ForSuffixRange()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, "/api/tracks/9101/stream");

        request.Headers.TryAddWithoutValidation(
            "Range", "bytes=-500");

        using var response = await _client.SendAsync(request);

        Assert.Equal(
            System.Net.HttpStatusCode.PartialContent,
            response.StatusCode);

        Assert.Equal(500, response.Content.Headers.ContentLength);

        Assert.Equal(
            _audioBytes.Length - 500,
            response.Content.Headers.ContentRange?.From);

        Assert.Equal(
            _audioBytes.Length - 1,
            response.Content.Headers.ContentRange?.To);

        var bytes = await response.Content.ReadAsByteArrayAsync();

        Assert.Equal(_audioBytes.TakeLast(500).ToArray(), bytes);
    }

        [Fact]
    public async Task StreamTrack_ReturnsOk_WithFullAudioContent()
    {
        using var response =
            await _client.GetAsync("/api/tracks/9101/stream");

        Assert.Equal(
            System.Net.HttpStatusCode.OK,
            response.StatusCode);

        Assert.Equal(
            "audio/mpeg",
            response.Content.Headers.ContentType?.MediaType);

        Assert.Equal(
            _audioBytes.Length,
            response.Content.Headers.ContentLength);

        var bytes = await response.Content.ReadAsByteArrayAsync();

        Assert.Equal(_audioBytes, bytes);
    }

    [Fact]
    public async Task StreamTrack_ReturnsBadRequest_ForZeroTrackId()
    {
        using var response =
            await _client.GetAsync("/api/tracks/0/stream");

        Assert.Equal(
            System.Net.HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Theory]
    [InlineData(9999)]
    [InlineData(9102)]
    [InlineData(9103)]
    public async Task StreamTrack_ReturnsNotFound_ForUnavailableAudio(
        ulong trackId)
    {
        using var response =
            await _client.GetAsync($"/api/tracks/{trackId}/stream");

        Assert.Equal(
            System.Net.HttpStatusCode.NotFound,
            response.StatusCode);
    }

   [Theory]
[InlineData(9102UL)]
[InlineData(9103UL)]
public async Task StreamTrack_DoesNotExposeInternalPaths_InErrorResponse(
    ulong trackId)
{
    using var response =
        await _client.GetAsync($"/api/tracks/{trackId}/stream");

    Assert.Equal(
        System.Net.HttpStatusCode.NotFound,
        response.StatusCode);

    var body = await response.Content.ReadAsStringAsync();

    Assert.DoesNotContain(_temporaryDirectory, body,
        StringComparison.OrdinalIgnoreCase);

    Assert.DoesNotContain("outside.mp3", body,
        StringComparison.OrdinalIgnoreCase);

    Assert.DoesNotContain("System.IO", body,
        StringComparison.OrdinalIgnoreCase);

    Assert.DoesNotContain("stack trace", body,
        StringComparison.OrdinalIgnoreCase);
} 
}