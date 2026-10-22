
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Soundwaves.Api.Data;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Models;

namespace Soundwaves.Api.Tests;

public class CatalogApiIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CatalogApiIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        var testFactory = factory.WithWebHostBuilder(builder =>
            builder.UseEnvironment("Testing"));

        _client = testFactory.CreateClient();

        // Seed test-only data in the InMemory database.
        using var scope = testFactory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<SoundwavesDbContext>();

        if (!db.Artists.Any(a => a.Id == 101))
        {
            db.Artists.Add(new Artist
            {
                Id = 101,
                Name = "Integration Test Artist",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!db.Albums.Any(a => a.Id == 101))
        {
            db.Albums.Add(new Album
            {
                Id = 101,
                ArtistId = 101,
                Title = "Integration Test Album",
                ReleaseDate = new DateOnly(2026, 10, 15),
                ArtworkPath = "/test/album.jpg",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!db.Tracks.Any(t => t.Id == 101))
        {
            db.Tracks.Add(new Track
            {
                Id = 101,
                ArtistId = 101,
                AlbumId = 101,
                Title = "Integration Test Track",
                DurationMs = 180000,
                TrackNumber = 1,
                FilePath = "/test/track.mp3",
                CreatedAt = DateTime.UtcNow
            });
        }

        db.SaveChanges();
    }

    [Theory]
    [InlineData("/api/albums/0")]
    [InlineData("/api/artists/0")]
    public async Task InvalidId_ReturnsBadRequest(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Theory]
    [InlineData("/api/albums/999999")]
    [InlineData("/api/artists/999999")]
    public async Task UnknownId_ReturnsNotFound(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task AlbumDetails_ReturnsOkWithCorrectData()
    {
        var response = await _client.GetAsync("/api/albums/101");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var album = await response.Content
            .ReadFromJsonAsync<AlbumDetailsResponse>();

        Assert.NotNull(album);
        Assert.Equal((ulong)101, album.Id);
        Assert.Equal("Integration Test Album", album.Title);
        Assert.NotNull(album.Artist);
        Assert.Equal("Integration Test Artist", album.Artist.Name);
        Assert.Contains(album.Tracks,
            track => track.Title == "Integration Test Track");
    }

    [Fact]
    public async Task ArtistDetails_ReturnsOkWithCorrectData()
    {
        var response = await _client.GetAsync("/api/artists/101");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var artist = await response.Content
            .ReadFromJsonAsync<ArtistDetailsResponse>();

        Assert.NotNull(artist);
        Assert.Equal((ulong)101, artist.Id);
        Assert.Equal("Integration Test Artist", artist.Name);
        Assert.Contains(artist.Albums,
            album => album.Title == "Integration Test Album");
        Assert.Contains(artist.Tracks,
            track => track.Title == "Integration Test Track");
    }
}
