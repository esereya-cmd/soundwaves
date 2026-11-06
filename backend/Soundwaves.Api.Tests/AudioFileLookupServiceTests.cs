
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Soundwaves.Api.Data;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Tests;

public class AudioFileLookupServiceTests
{
    private static SoundwavesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SoundwavesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SoundwavesDbContext(options);
    }

    private static AudioFileLookupService CreateService(
        SoundwavesDbContext dbContext,
        string contentRoot)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MediaStorage:RootDirectory"] = "media"
            })
            .Build();

        var environment = new TestWebHostEnvironment
        {
            ContentRootPath = contentRoot
        };

        return new AudioFileLookupService(
            dbContext,
            configuration,
            environment);
    }

    private static async Task AddTrackAsync(
        SoundwavesDbContext dbContext,
        ulong id,
        string filePath)
    {
        dbContext.Tracks.Add(new Track
        {
            Id = id,
            Title = "Test Track",
            FilePath = filePath,
            MimeType = "audio/mpeg",
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAudioFilePathAsync_RejectsSymbolicLinkOutsideMediaRoot()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var mediaRoot = Path.Combine(directory.Path, "media");
        Directory.CreateDirectory(mediaRoot);

        // Create an MP3 outside the approved media directory.
        var outsideFile = Path.Combine(directory.Path, "outside.mp3");

        await File.WriteAllBytesAsync(
            outsideFile,
            new byte[] { 1, 2, 3 });

        // Create a symbolic link inside media pointing outside.
        var linkedFile = Path.Combine(mediaRoot, "linked.mp3");

        File.CreateSymbolicLink(linkedFile, outsideFile);

        await AddTrackAsync(dbContext, 1, "linked.mp3");

        var service = CreateService(dbContext, directory.Path);

        // Act
        var result = await service.GetAudioFilePathAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAudioFilePathAsync_ReturnsPath_WhenFileExists()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var mediaRoot = Path.Combine(directory.Path, "media");
        Directory.CreateDirectory(mediaRoot);

        var audioPath = Path.Combine(mediaRoot, "test.mp3");

        await File.WriteAllBytesAsync(
            audioPath,
            new byte[] { 1, 2, 3 });

        await AddTrackAsync(dbContext, 1, "test.mp3");

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(1);

        Assert.Equal(audioPath, result);
    }

    [Fact]
    public async Task GetAudioFilePathAsync_ReturnsNull_WhenTrackDoesNotExist()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAudioFilePathAsync_ReturnsNull_WhenTrackIdIsZero()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(0);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAudioFilePathAsync_ReturnsNull_WhenFileIsMissing()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        await AddTrackAsync(dbContext, 1, "missing.mp3");

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAudioFilePathAsync_ReturnsNull_WhenPathIsEmpty()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        await AddTrackAsync(dbContext, 1, "");

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(1);

        Assert.Null(result);
    }

    [Theory]
    [InlineData("../outside.mp3")]
    [InlineData("../../outside.mp3")]
    [InlineData("folder/../../../outside.mp3")]
    [InlineData(@"..\outside.mp3")]
    public async Task GetAudioFilePathAsync_RejectsDirectoryTraversal(
        string filePath)
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        await AddTrackAsync(dbContext, 1, filePath);

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAudioFilePathAsync_RejectsAbsolutePaths()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var outsidePath = Path.Combine(
            directory.Path,
            "outside.mp3");

        await File.WriteAllBytesAsync(
            outsidePath,
            new byte[] { 1, 2, 3 });

        await AddTrackAsync(dbContext, 1, outsidePath);

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAudioFilePathAsync_RejectsNonMp3Files()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var mediaRoot = Path.Combine(directory.Path, "media");
        Directory.CreateDirectory(mediaRoot);

        await File.WriteAllTextAsync(
            Path.Combine(mediaRoot, "test.txt"),
            "Not an MP3");

        await AddTrackAsync(dbContext, 1, "test.txt");

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(1);

        Assert.Null(result);
    }

    // SCRUM-135: Verify valid MP3 lookup in nested directories.
    [Fact]
    public async Task GetAudioFilePathAsync_ReturnsPath_ForNestedMp3()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var nestedDirectory = Path.Combine(
            directory.Path,
            "media",
            "albums");

        Directory.CreateDirectory(nestedDirectory);

        var audioPath = Path.Combine(
            nestedDirectory,
            "song.mp3");

        await File.WriteAllBytesAsync(
            audioPath,
            new byte[] { 1, 2, 3 });

        await AddTrackAsync(
            dbContext,
            1,
            "albums/song.mp3");

        var service = CreateService(dbContext, directory.Path);

        var result = await service.GetAudioFilePathAsync(1);

        Assert.Equal(audioPath, result);
    }

    // SCRUM-135: Verify missing media configuration is rejected.
    [Fact]
    public void AudioFileLookupService_Throws_WhenMediaRootNotConfigured()
    {
        using var dbContext = CreateDbContext();
        using var directory = new TemporaryDirectory();

        var configuration = new ConfigurationBuilder()
            .Build();

        var environment = new TestWebHostEnvironment
        {
            ContentRootPath = directory.Path
        };

        Assert.Throws<InvalidOperationException>(() =>
            new AudioFileLookupService(
                dbContext,
                configuration,
                environment));
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            "SoundwavesTests",
            Guid.NewGuid().ToString("N"));

        public TemporaryDirectory()
        {
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(
                    Path,
                    recursive: true);
            }
        }
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Soundwaves.Api";

        public string EnvironmentName { get; set; } = "Testing";

        public string WebRootPath { get; set; } = string.Empty;

        public IFileProvider WebRootFileProvider { get; set; } =
            new NullFileProvider();

        public string ContentRootPath { get; set; } = string.Empty;

        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }
}
