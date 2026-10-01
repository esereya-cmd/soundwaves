using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class MusicCatalogServiceTests
{
    [Fact]
    public void GetTrackById_ReturnsTrack_WhenTrackExists()
    {
        // Arrange
        var service = new MusicCatalogService();

        // Act
        var result = service.GetTrackById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal((ulong)1, result.Id);
    }

    [Fact]
    public void GetTrackById_ReturnsNull_WhenTrackDoesNotExist()
    {
        // Arrange
        var service = new MusicCatalogService();

        // Act
        var result = service.GetTrackById(999);

        // Assert
        Assert.Null(result);
    }
}