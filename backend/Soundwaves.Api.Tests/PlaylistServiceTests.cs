
using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class PlaylistServiceTests
{
    private static SoundwavesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SoundwavesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SoundwavesDbContext(options);
    }

    [Fact]
    public async Task CreatePlaylistAsync_CreatesPlaylist()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "My Favorites",
            Description = "Favorite songs"
        };

        var result = await service.CreatePlaylistAsync(1001, request);

        Assert.Equal("My Favorites", result.Name);
        Assert.Equal("Favorite songs", result.Description);
        Assert.Empty(result.Tracks);

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal((ulong)1001, saved.UserId);
        Assert.Equal("My Favorites", saved.Name);
    }

    [Fact]
    public async Task CreatePlaylistAsync_TrimsPlaylistName()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "  Study Music  "
        };

        var result = await service.CreatePlaylistAsync(1001, request);

        Assert.Equal("Study Music", result.Name);

        var saved = await dbContext.Playlists.SingleAsync();

        Assert.Equal("Study Music", saved.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreatePlaylistAsync_RejectsEmptyNames(
        string name)
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = name
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(1001, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task CreatePlaylistAsync_RejectsLongName()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = new string('A', 256)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(1001, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task CreatePlaylistAsync_RejectsLongDescription()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "Test Playlist",
            Description = new string('A', 1001)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(1001, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task CreatePlaylistAsync_RejectsInvalidUserId()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var request = new CreatePlaylistRequest
        {
            Name = "Test Playlist"
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreatePlaylistAsync(0, request));

        Assert.Empty(dbContext.Playlists);
    }

    [Fact]
    public async Task GetPlaylistsAsync_ReturnsOnlyCurrentUsersPlaylists()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.AddRange(
            new Playlist
            {
                Id = 1,
                UserId = 1001,
                Name = "My Playlist"
            },
            new Playlist
            {
                Id = 2,
                UserId = 2002,
                Name = "Another User Playlist"
            }
        );

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var results = (await service.GetPlaylistsAsync(1001)).ToList();

        Assert.Single(results);
        Assert.Equal("My Playlist", results[0].Name);
        Assert.Equal((ulong)1, results[0].Id);
    }

    [Fact]
    public async Task GetPlaylistsAsync_ReturnsEmptyWhenNoPlaylistsExist()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        var results = await service.GetPlaylistsAsync(1001);

        Assert.Empty(results);
    }

    [Fact]
    public async Task GetPlaylistsAsync_RejectsInvalidUserId()
    {
        using var dbContext = CreateDbContext();
        var service = new PlaylistService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetPlaylistsAsync(0));
    }

    [Fact]
    public async Task GetPlaylistsAsync_ReturnsPlaylistsInIdOrder()
    {
        using var dbContext = CreateDbContext();

        dbContext.Playlists.AddRange(
            new Playlist
            {
                Id = 3,
                UserId = 1001,
                Name = "Third"
            },
            new Playlist
            {
                Id = 1,
                UserId = 1001,
                Name = "First"
            },
            new Playlist
            {
                Id = 2,
                UserId = 1001,
                Name = "Second"
            }
        );

        await dbContext.SaveChangesAsync();

        var service = new PlaylistService(dbContext);

        var results = (await service.GetPlaylistsAsync(1001)).ToList();

        Assert.Equal(
            new ulong[] { 1, 2, 3 },
            results.Select(p => p.Id));
    }
}
