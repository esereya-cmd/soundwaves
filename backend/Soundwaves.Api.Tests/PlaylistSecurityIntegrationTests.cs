
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

public class PlaylistSecurityIntegrationTests : IDisposable
{
    private const ulong OwnerId = 1001;
    private const ulong OtherUserId = 2002;
    private const ulong TrackId = 92001;

    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestCurrentUserService _identity;

    private sealed class TestCurrentUserService : ICurrentUserService
    {
        public ulong? UserId { get; set; } = OwnerId;

        public ulong? GetCurrentUserId() => UserId;
    }

    public PlaylistSecurityIntegrationTests()
    {
        var databaseName =
            "PlaylistSecurity_" + Guid.NewGuid().ToString("N");

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

                    services.AddSingleton<TestCurrentUserService>();
                    services.AddSingleton<ICurrentUserService>(
                        sp => sp.GetRequiredService<
                            TestCurrentUserService>());
                });
            });

        _client = _factory.CreateClient();

        _identity = _factory.Services
            .GetRequiredService<TestCurrentUserService>();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<SoundwavesDbContext>();

        db.Tracks.Add(new Track
        {
            Id = TrackId,
            Title = "Security Integration Test Track",
            FilePath = "/test/security.mp3",
            MimeType = "audio/mpeg",
            CreatedAt = DateTime.UtcNow
        });

        db.SaveChanges();
    }

    private async Task<ulong> CreateOwnerPlaylistAsync()
    {
        _identity.UserId = OwnerId;

        var response = await _client.PostAsJsonAsync(
            "/api/playlists",
            new
            {
                name = "Owner Security Playlist",
                description = "SCRUM-204 security testing"
            });

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        return body.GetProperty("id").GetUInt64();
    }

    private async Task AddOwnerTrackAsync(ulong playlistId)
    {
        _identity.UserId = OwnerId;

        var response = await _client.PostAsJsonAsync(
            $"/api/playlists/{playlistId}/tracks",
            new { trackId = TrackId });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<HttpResponseMessage> ExecuteAsync(
        string operation, ulong playlistId)
    {
        var url = $"/api/playlists/{playlistId}";

        return operation switch
        {
            "get" => await _client.GetAsync(url),

            "update" => await _client.PutAsJsonAsync(
                url,
                new
                {
                    name = "Unauthorized Rename",
                    description = "Should not be saved"
                }),

            "delete" => await _client.DeleteAsync(url),

            "add" => await _client.PostAsJsonAsync(
                $"{url}/tracks",
                new { trackId = TrackId }),

            "remove" => await _client.DeleteAsync(
                $"{url}/tracks/{TrackId}"),

            "reorder" => await _client.PutAsJsonAsync(
                $"{url}/tracks/reorder",
                new { trackIds = new ulong[] { TrackId } }),

            _ => throw new ArgumentException(
                $"Unknown operation: {operation}")
        };
    }

    [Fact]
    public async Task UnauthenticatedRequests_Return401()
    {
        var playlistId = await CreateOwnerPlaylistAsync();

        _identity.UserId = null;

        using var create = await _client.PostAsJsonAsync(
            "/api/playlists",
            new { name = "Unauthorized Playlist" });

        Assert.Equal(HttpStatusCode.Unauthorized,
            create.StatusCode);

        using var list = await _client.GetAsync(
            "/api/playlists");

        Assert.Equal(HttpStatusCode.Unauthorized,
            list.StatusCode);

        foreach (var operation in new[]
        {
            "get", "update", "delete",
            "add", "remove", "reorder"
        })
        {
            using var response = await ExecuteAsync(
                operation, playlistId);

            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        _identity.UserId = OwnerId;

        using var verify = await _client.GetAsync(
            $"/api/playlists/{playlistId}");

        Assert.Equal(HttpStatusCode.OK, verify.StatusCode);
    }

    [Theory]
    [InlineData("get")]
    [InlineData("update")]
    [InlineData("delete")]
    [InlineData("add")]
    [InlineData("remove")]
    [InlineData("reorder")]
    public async Task OtherUser_CannotAccessOrModifyPlaylist(
        string operation)
    {
        var playlistId = await CreateOwnerPlaylistAsync();

        // Set up a track association when necessary.
        if (operation is "remove" or "reorder")
        {
            await AddOwnerTrackAsync(playlistId);
        }

        int originalTrackCount;

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<SoundwavesDbContext>();

            originalTrackCount = await db.PlaylistTracks
                .CountAsync(pt => pt.PlaylistId == playlistId);
        }

        // Simulate a different user attempting access.
        _identity.UserId = OtherUserId;

        using var deniedResponse = await ExecuteAsync(
            operation, playlistId);

        Assert.Equal(
            HttpStatusCode.NotFound,
            deniedResponse.StatusCode);

        // Verify denied operations did not modify owner data.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<SoundwavesDbContext>();

            var playlist = await db.Playlists
                .AsNoTracking()
                .SingleAsync(p => p.Id == playlistId);

            Assert.Equal(OwnerId, playlist.UserId);
            Assert.Equal(
                "Owner Security Playlist",
                playlist.Name);

            var trackCount = await db.PlaylistTracks
                .CountAsync(pt => pt.PlaylistId == playlistId);

            Assert.Equal(originalTrackCount, trackCount);
        }

        // Owner must still be able to access the playlist.
        _identity.UserId = OwnerId;

        using var ownerResponse = await _client.GetAsync(
            $"/api/playlists/{playlistId}");

        Assert.Equal(
            HttpStatusCode.OK,
            ownerResponse.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
