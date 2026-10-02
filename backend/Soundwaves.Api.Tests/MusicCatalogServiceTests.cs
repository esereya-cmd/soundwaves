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
}