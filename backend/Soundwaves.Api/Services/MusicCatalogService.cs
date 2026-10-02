using Soundwaves.Api.Models;
using Soundwaves.Api.Data;

namespace Soundwaves.Api.Services;

public class MusicCatalogService : IMusicCatalogService
{
    private readonly List<Artist> _artists = new()
    {
        new Artist
        {
            Id = 1,
            Name = "Sample Artist",
            CreatedAt = DateTime.UtcNow
        }
    };

    private readonly List<Album> _albums = new()
    {
        new Album
        {
            Id = 1,
            ArtistId = 1,
            Title = "Sample Album",
            ReleaseDate = new DateOnly(2026, 1, 1),
            CreatedAt = DateTime.UtcNow
        }
    };

    private readonly List<Track> _tracks = new()
    {
        new Track
        {
            Id = 1,
            AlbumId = 1,
            ArtistId = 1,
            Title = "Sample Track",
            DurationMs = 180000,
            TrackNumber = 1,
            FilePath = "/music/sample-track.mp3",
            MimeType = "audio/mpeg",
            CreatedAt = DateTime.UtcNow
        }
    };

    private readonly SoundwavesDbContext _dbContext;

    public MusicCatalogService(SoundwavesDbContext dbContext)
    {
    _dbContext = dbContext;
    }

    public IEnumerable<Track> GetTracks()
    {
        return _tracks;
    }

    public Track? GetTrackById(ulong id)
    {
        return _dbContext.Tracks.FirstOrDefault(track => track.Id == id);
    }

    public IEnumerable<Artist> GetArtists()
    {
        return _artists;
    }

    public IEnumerable<Album> GetAlbums()
    {
        return _albums;
    }
    public IEnumerable<Track> SearchTracks(string query)
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return Enumerable.Empty<Track>();
    }

    return _tracks.Where(track =>
        track.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        _artists.Any(artist =>
            artist.Id == track.ArtistId &&
            artist.Name.Contains(query, StringComparison.OrdinalIgnoreCase)));
}
}