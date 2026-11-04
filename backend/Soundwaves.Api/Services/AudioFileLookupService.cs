
using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;

namespace Soundwaves.Api.Services;

public class AudioFileLookupService : IAudioFileLookupService
{
    private readonly SoundwavesDbContext _dbContext;
    private readonly string _mediaRoot;

    public AudioFileLookupService(
        SoundwavesDbContext dbContext,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _dbContext = dbContext;

        var configuredRoot =
            configuration["MediaStorage:RootDirectory"];

        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            throw new InvalidOperationException(
                "Media storage directory is not configured.");
        }

        _mediaRoot = Path.GetFullPath(
            configuredRoot,
            environment.ContentRootPath);
    }

    public async Task<string?> GetAudioFilePathAsync(ulong trackId)
    {
        if (trackId == 0)
        {
            return null;
        }

        var track = await _dbContext.Tracks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == trackId);

        if (track == null ||
            string.IsNullOrWhiteSpace(track.FilePath))
        {
            return null;
        }

        var relativePath = track.FilePath;

        // Reject rooted paths, traversal, and backslash separators.
        if (Path.IsPathRooted(relativePath) ||
            Path.IsPathFullyQualified(relativePath) ||
            relativePath.Contains('\\') ||
            relativePath.Split('/').Contains(".."))
        {
            return null;
        }

        // Only MP3 files are supported.
        if (!string.Equals(
            Path.GetExtension(relativePath),
            ".mp3",
            StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        string fullPath;

        try
        {
            fullPath = Path.GetFullPath(
                relativePath,
                _mediaRoot);
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
        catch (PathTooLongException)
        {
            return null;
        }

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var rootWithSeparator =
            Path.TrimEndingDirectorySeparator(_mediaRoot)
            + Path.DirectorySeparatorChar;

        // Prevent access outside the configured media directory.
        if (!fullPath.StartsWith(rootWithSeparator, comparison))
        {
            return null;
        }

        try
        {
            // Reject symbolic links and reparse points in the path.
            var currentPath = fullPath;

            while (!string.Equals(
                currentPath,
                _mediaRoot,
                comparison))
            {
                if (HasLinkOrReparsePoint(currentPath))
                {
                    return null;
                }

                var parent = Directory.GetParent(currentPath);

                if (parent == null)
                {
                    return null;
                }

                currentPath = parent.FullName;
            }

            // Also validate the configured media root.
            if (HasLinkOrReparsePoint(_mediaRoot))
            {
                return null;
            }

            if (!File.Exists(fullPath))
            {
                return null;
            }

            return fullPath;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static bool HasLinkOrReparsePoint(string path)
    {
        FileSystemInfo entry = Directory.Exists(path)
            ? new DirectoryInfo(path)
            : new FileInfo(path);

        entry.Refresh();

        // LinkTarget can identify links even when their targets
        // do not exist.
        if (entry.LinkTarget != null)
        {
            return true;
        }

        if (entry.Exists &&
            (entry.Attributes & FileAttributes.ReparsePoint) != 0)
        {
            return true;
        }

        return false;
    }
}
