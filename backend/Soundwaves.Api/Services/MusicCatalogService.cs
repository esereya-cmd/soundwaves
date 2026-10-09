using Soundwaves.Api.Models;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<Artist>> GetArtistsAsync()
    {
        return await _dbContext.Artists
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Album>> GetAlbumsAsync()
        {
            return await _dbContext.Albums
                .AsNoTracking()
                .ToListAsync();
        }
    public async Task<Album?> GetAlbumByIdAsync(ulong id)
    {
        return await _dbContext.Albums
            .AsNoTracking()
            .FirstOrDefaultAsync(album => album.Id == id);
    }

    public async Task<Artist?> GetArtistByIdAsync(ulong id)
    {
        return await _dbContext.Artists
            .AsNoTracking()
            .FirstOrDefaultAsync(artist => artist.Id == id);
    }
    public async Task<IEnumerable<Album>> GetAlbumsByArtistIdAsync(ulong artistId)
    {
        return await _dbContext.Albums
            .AsNoTracking()
            .Where(album => album.ArtistId == artistId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Track>> GetTracksByAlbumIdAsync(ulong albumId)
    {
        return await _dbContext.Tracks
            .AsNoTracking()
            .Where(track => track.AlbumId == albumId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Track>> GetTracksByArtistIdAsync(ulong artistId)
    {
        return await _dbContext.Tracks
            .AsNoTracking()
            .Where(track => track.ArtistId == artistId)
            .ToListAsync();
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