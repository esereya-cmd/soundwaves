
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soundwaves.Api.Data;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class PlaylistApiIntegrationTests : IDisposable
{
    private const ulong TestTrackId = 91001;

    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PlaylistApiIntegrationTests()
    {
        var databaseName =
            "SoundwavesPlaylistHttp_" + Guid.NewGuid().ToString("N");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<
                        DbContextOptions<SoundwavesDbContext>>();

                    services.AddDbContext<SoundwavesDbContext>(
                        options => options.UseInMemoryDatabase(
                            databaseName));

                    services.RemoveAll<ICurrentUserService>();
                    services.AddScoped<
                        ICurrentUserService, TestCurrentUserService>();
                });
            });

        _client = _factory.CreateClient();

        // Seed a real track in the test-only database.
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<SoundwavesDbContext>();

        db.Tracks.Add(new Track
        {
            Id = TestTrackId,
            Title = "SCRUM-203 HTTP Test Track",
            FilePath = "/test/scrum-203.mp3",
            MimeType = "audio/mpeg",
            CreatedAt = DateTime.UtcNow
        });

        db.SaveChanges();
    }

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public ulong? GetCurrentUserId() => 1001;
    }

    private async Task<ulong> CreatePlaylistAsync()
    {
        var name = $"HTTP Integration {Guid.NewGuid():N}";

        var response = await _client.PostAsJsonAsync(
            "/api/playlists",
            new
            {
                name,
                description = "Sprint 5 automated test"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(name, body.GetProperty("name").GetString());

        return body.GetProperty("id").GetUInt64();
    }

    // Existing SCRUM-202 tests

    [Fact]
    public async Task CreatePlaylist_Returns201()
    {
        var id = await CreatePlaylistAsync();
        Assert.True(id > 0);
    }

    [Fact]
    public async Task GetPlaylists_ReturnsCreatedPlaylist()
    {
        var id = await CreatePlaylistAsync();

        var response = await _client.GetAsync("/api/playlists");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var playlists = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Contains(
            playlists.EnumerateArray(),
            item => item.GetProperty("id").GetUInt64() == id);
    }

    [Fact]
    public async Task GetPlaylistById_Returns200()
    {
        var id = await CreatePlaylistAsync();

        var response = await _client.GetAsync(
            $"/api/playlists/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(id, body.GetProperty("id").GetUInt64());
    }

    [Fact]
    public async Task UpdatePlaylist_ReturnsUpdatedData()
    {
        var id = await CreatePlaylistAsync();

        var response = await _client.PutAsJsonAsync(
            $"/api/playlists/{id}",
            new
            {
                name = "Updated HTTP Integration Playlist",
                description = "Updated in automated test"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/playlists/{id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var body = await getResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Updated HTTP Integration Playlist",
            body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetMissingPlaylist_Returns404()
    {
        var response = await _client.GetAsync(
            "/api/playlists/999999999");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task CreatePlaylist_EmptyName_Returns400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/playlists",
            new
            {
                name = "",
                description = "Invalid input"
            });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    // SCRUM-203: Playlist Add/Remove Track HTTP tests

    [Fact]
    public async Task AddTrack_ValidTrack_Returns200()
    {
        var playlistId = await CreatePlaylistAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/tracks",
            new { trackId = TestTrackId });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify that the playlist still exists after adding.
        var getResponse = await _client.GetAsync(
            $"/api/playlists/{playlistId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task AddTrack_InvalidTrackId_Returns400()
    {
        var playlistId = await CreatePlaylistAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/tracks",
            new { trackId = 0 });

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task AddTrack_MissingPlaylist_Returns404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/playlists/999999999/tracks",
            new { trackId = TestTrackId });

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task AddTrack_MissingTrack_Returns404()
    {
        var playlistId = await CreatePlaylistAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/tracks",
            new { trackId = 999999999UL });

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task AddTrack_DuplicateTrack_Returns409()
    {
        var playlistId = await CreatePlaylistAsync();

        var firstResponse = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/tracks",
            new { trackId = TestTrackId });

        Assert.Equal(
            HttpStatusCode.OK,
            firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/tracks",
            new { trackId = TestTrackId });

        Assert.Equal(
            HttpStatusCode.Conflict,
            duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task RemoveTrack_ExistingTrack_Returns204()
    {
        var playlistId = await CreatePlaylistAsync();

        var addResponse = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/tracks",
            new { trackId = TestTrackId });

        Assert.Equal(
            HttpStatusCode.OK,
            addResponse.StatusCode);

        var removeResponse = await _client.DeleteAsync(
            $"/api/playlists/{playlistId}/tracks/{TestTrackId}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            removeResponse.StatusCode);

        // Removing it again must fail because the link is gone.
        var secondResponse = await _client.DeleteAsync(
            $"/api/playlists/{playlistId}/tracks/{TestTrackId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            secondResponse.StatusCode);
    }

    [Fact]
    public async Task RemoveTrack_MissingAssociation_Returns404()
    {
        var playlistId = await CreatePlaylistAsync();

        var response = await _client.DeleteAsync(
            $"/api/playlists/{playlistId}/tracks/{TestTrackId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
