
using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class CatalogSearchTests
{
    private static SoundwavesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SoundwavesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SoundwavesDbContext(options);
    }

    [Fact]
    public async Task SearchCatalogAsync_ReturnsMatchingTrack()
    {
        using var db = CreateDbContext();

        db.Tracks.AddRange(
            new Track { Id = 1, Title = "Midnight Drive", FilePath = "/a.mp3" },
            new Track { Id = 2, Title = "Morning Light", FilePath = "/b.mp3" }
        );
        await db.SaveChangesAsync();

        var service = new MusicCatalogService(db);

        var result = await service.SearchCatalogAsync("Midnight");

        Assert.Single(result.Tracks);
        Assert.Equal("Midnight Drive", result.Tracks.Single().Title);
        Assert.Empty(result.Albums);
        Assert.Empty(result.Artists);
    }

    [Fact]
    public async Task SearchCatalogAsync_ReturnsMatchingAlbum()
    {
        using var db = CreateDbContext();

        db.Albums.AddRange(
            new Album { Id = 1, ArtistId = 1, Title = "Ocean Dreams" },
            new Album { Id = 2, ArtistId = 1, Title = "City Nights" }
        );
        await db.SaveChangesAsync();

        var service = new MusicCatalogService(db);

        var result = await service.SearchCatalogAsync("Ocean");

        Assert.Single(result.Albums);
        Assert.Equal("Ocean Dreams", result.Albums.Single().Title);
        Assert.Empty(result.Tracks);
        Assert.Empty(result.Artists);
    }

    [Fact]
    public async Task SearchCatalogAsync_ReturnsMatchingArtist()
    {
        using var db = CreateDbContext();

        db.Artists.AddRange(
            new Artist { Id = 1, Name = "Silver Echo" },
            new Artist { Id = 2, Name = "Golden Waves" }
        );
        await db.SaveChangesAsync();

        var service = new MusicCatalogService(db);

        var result = await service.SearchCatalogAsync("Silver");

        Assert.Single(result.Artists);
        Assert.Equal("Silver Echo", result.Artists.Single().Name);
        Assert.Empty(result.Tracks);
        Assert.Empty(result.Albums);
    }

    [Fact]
    public async Task SearchCatalogAsync_ReturnsAllMatchingCategories()
    {
        using var db = CreateDbContext();

        db.Tracks.Add(new Track
        {
            Id = 1,
            Title = "Blue Sky",
            FilePath = "/blue.mp3"
        });

        db.Albums.Add(new Album
        {
            Id = 1,
            ArtistId = 1,
            Title = "Blue Horizon"
        });

        db.Artists.Add(new Artist
        {
            Id = 1,
            Name = "Blue Waves"
        });

        await db.SaveChangesAsync();

        var service = new MusicCatalogService(db);

        var result = await service.SearchCatalogAsync("Blue");

        Assert.Single(result.Tracks);
        Assert.Single(result.Albums);
        Assert.Single(result.Artists);
    }

    [Fact]
    public async Task SearchCatalogAsync_ReturnsEmpty_WhenNoMatches()
    {
        using var db = CreateDbContext();
        var service = new MusicCatalogService(db);

        var result = await service.SearchCatalogAsync("Nonexistent");

        Assert.Empty(result.Tracks);
        Assert.Empty(result.Albums);
        Assert.Empty(result.Artists);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task SearchCatalogAsync_ReturnsEmpty_ForBlankQueries(
        string query)
    {
        using var db = CreateDbContext();
        var service = new MusicCatalogService(db);

        var result = await service.SearchCatalogAsync(query);

        Assert.Empty(result.Tracks);
        Assert.Empty(result.Albums);
        Assert.Empty(result.Artists);
    }

    [Fact]
    public async Task SearchCatalogAsync_TrimsWhitespace()
    {
        using var db = CreateDbContext();

        db.Tracks.Add(new Track
        {
            Id = 1,
            Title = "Sunset Road",
            FilePath = "/sunset.mp3"
        });

        await db.SaveChangesAsync();

        var service = new MusicCatalogService(db);

        var result = await service.SearchCatalogAsync("  Sunset  ");

        Assert.Single(result.Tracks);
        Assert.Equal("Sunset Road", result.Tracks.Single().Title);
    }
}
