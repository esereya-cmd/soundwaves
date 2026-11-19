using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class PlaylistApiIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PlaylistApiIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICurrentUserService>();
                    services.AddScoped<ICurrentUserService, TestCurrentUserService>();
                });
            });

        _client = _factory.CreateClient();
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
            new { name, description = "Sprint 5 automated test" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(name, body.GetProperty("name").GetString());

        return body.GetProperty("id").GetUInt64();
    }

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

        Assert.Contains(playlists.EnumerateArray(),
            item => item.GetProperty("id").GetUInt64() == id);
    }

    [Fact]
    public async Task GetPlaylistById_Returns200()
    {
        var id = await CreatePlaylistAsync();

        var response = await _client.GetAsync($"/api/playlists/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
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

        var getResponse = await _client.GetAsync($"/api/playlists/{id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var body = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Updated HTTP Integration Playlist",
            body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetMissingPlaylist_Returns404()
    {
        var response = await _client.GetAsync(
            "/api/playlists/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatePlaylist_EmptyName_Returns400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/playlists",
            new { name = "", description = "Invalid input" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
