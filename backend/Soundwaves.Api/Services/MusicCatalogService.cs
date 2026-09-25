using Soundwaves.Api.Models;

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

    public IEnumerable<Track> GetTracks()
    {
        return _tracks;
    }

    public Track? GetTrackById(ulong id)
    {
        return _tracks.FirstOrDefault(track => track.Id == id);
    }

    public IEnumerable<Artist> GetArtists()
    {
        return _artists;
    }

    public IEnumerable<Album> GetAlbums()
    {
        return _albums;
    }
}