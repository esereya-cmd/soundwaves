using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class MusicCatalogServiceTests
{
    private SoundwavesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SoundwavesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new SoundwavesDbContext(options);
    }
    [Fact]
    public async Task GetAlbumByIdAsync_ReturnsAlbum_WhenAlbumExists()
    {
        // Arrange
        using var dbContext = CreateDbContext();

        dbContext.Albums.Add(new Album
        {
            Id = 1,
            ArtistId = 1,
            Title = "Test Album",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var service = new MusicCatalogService(dbContext);

        // Act
        var result = await service.GetAlbumByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Album", result.Title);
    }

    [Fact]
    public async Task GetAlbumByIdAsync_ReturnsNull_WhenAlbumDoesNotExist()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var service = new MusicCatalogService(dbContext);

        // Act
        var result = await service.GetAlbumByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetTrackById_ReturnsTrack_WhenTrackExists()
    {
        // Arrange
        using var dbContext = CreateDbContext();

        dbContext.Tracks.Add(new Track
        {
            Id = 1,
            Title = "Test Track",
            FilePath = "/test/test-track.mp3",
            CreatedAt = DateTime.UtcNow
        });

        dbContext.SaveChanges();

        var service = new MusicCatalogService(dbContext);

        // Act
        var result = service.GetTrackById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((ulong)1, result.Id);
        Assert.Equal("Test Track", result.Title);
    }

    [Fact]
    public void GetTrackById_ReturnsNull_WhenTrackDoesNotExist()
    {
        // Arrange
        using var dbContext = CreateDbContext();

        var service = new MusicCatalogService(dbContext);

        // Act
        var result = service.GetTrackById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetArtistByIdAsync_ReturnsArtist_WhenArtistExists()
    {
        // Arrange
        using var dbContext = CreateDbContext();

        dbContext.Artists.Add(new Artist
        {
            Id = 1,
            Name = "Test Artist",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        var service = new MusicCatalogService(dbContext);

        // Act
        var result = await service.GetArtistByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Artist", result.Name);
    }

    [Fact]
    public async Task GetArtistByIdAsync_ReturnsNull_WhenArtistDoesNotExist()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var service = new MusicCatalogService(dbContext);

        // Act
        var result = await service.GetArtistByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAlbumsByArtistIdAsync_ReturnsOnlyMatchingAlbums()
    {
        using var dbContext = CreateDbContext();

        dbContext.Albums.AddRange(
            new Album { Id = 1, ArtistId = 1, Title = "Album A" },
            new Album { Id = 2, ArtistId = 2, Title = "Album B" }
        );

        await dbContext.SaveChangesAsync();

        var service = new MusicCatalogService(dbContext);

        var results = (await service.GetAlbumsByArtistIdAsync(1)).ToList();

        Assert.Single(results);
        Assert.Equal("Album A", results[0].Title);
    }

    [Fact]
    public async Task GetTracksByAlbumIdAsync_ReturnsOnlyMatchingTracks()
    {
        using var dbContext = CreateDbContext();

        dbContext.Tracks.AddRange(
            new Track { Id = 1, AlbumId = 1, Title = "Track A", FilePath = "/a.mp3" },
            new Track { Id = 2, AlbumId = 2, Title = "Track B", FilePath = "/b.mp3" }
        );

        await dbContext.SaveChangesAsync();

        var service = new MusicCatalogService(dbContext);

        var results = (await service.GetTracksByAlbumIdAsync(1)).ToList();

        Assert.Single(results);
        Assert.Equal("Track A", results[0].Title);
    }
    [Fact]
    public async Task GetTracksByArtistIdAsync_ReturnsOnlyMatchingTracks()
    {
        using var dbContext = CreateDbContext();

        dbContext.Tracks.AddRange(
            new Track { Id = 1, ArtistId = 1, Title = "Track A", FilePath = "/a.mp3" },
            new Track { Id = 2, ArtistId = 2, Title = "Track B", FilePath = "/b.mp3" }
        );

        await dbContext.SaveChangesAsync();

        var service = new MusicCatalogService(dbContext);

        var results = (await service.GetTracksByArtistIdAsync(1)).ToList();

        Assert.Single(results);
        Assert.Equal("Track A", results[0].Title);
    }

    [Fact]
    public async Task GetAlbumsAsync_ReturnsAllAlbums()
    {
        using var dbContext = CreateDbContext();

        dbContext.Albums.AddRange(
            new Album { Id = 1, ArtistId = 1, Title = "Album A" },
            new Album { Id = 2, ArtistId = 1, Title = "Album B" }
        );

        await dbContext.SaveChangesAsync();

        var service = new MusicCatalogService(dbContext);

        var results = (await service.GetAlbumsAsync()).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, album => album.Title == "Album A");
        Assert.Contains(results, album => album.Title == "Album B");
    }

    [Fact]
    public async Task GetArtistsAsync_ReturnsAllArtists()
    {
        using var dbContext = CreateDbContext();

        dbContext.Artists.AddRange(
            new Artist { Id = 1, Name = "Artist A" },
            new Artist { Id = 2, Name = "Artist B" }
        );

        await dbContext.SaveChangesAsync();

        var service = new MusicCatalogService(dbContext);

        var results = (await service.GetArtistsAsync()).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, artist => artist.Name == "Artist A");
        Assert.Contains(results, artist => artist.Name == "Artist B");
    }

    [Fact]
    public async Task GetAlbumsByArtistIdAsync_ReturnsEmpty_WhenNoAlbumsMatch()
    {
        using var dbContext = CreateDbContext();
        var service = new MusicCatalogService(dbContext);

        var results = await service.GetAlbumsByArtistIdAsync(999);

        Assert.Empty(results);
    }
}
