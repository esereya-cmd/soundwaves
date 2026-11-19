
using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Models;

namespace Soundwaves.Api.Services;

public class PlaylistService : IPlaylistService
{
    private readonly SoundwavesDbContext _dbContext;

    public PlaylistService(SoundwavesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlaylistCreatedResponse> CreatePlaylistAsync(
        ulong userId,
        CreatePlaylistRequest request)
    {
        if (userId == 0)
        {
            throw new ArgumentException(
                "A valid user ID is required.",
                nameof(userId));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        string name = request.Name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Playlist name cannot be empty.");
        }

        if (name.Length > 255)
        {
            throw new ArgumentException(
                "Playlist name cannot exceed 255 characters.");
        }

        if (request.Description?.Length > 1000)
        {
            throw new ArgumentException(
                "Playlist description cannot exceed 1000 characters.");
        }

        var playlist = new Playlist
        {
            UserId = userId,
            Name = name,
            Description = request.Description
        };

        _dbContext.Playlists.Add(playlist);

        await _dbContext.SaveChangesAsync();

        return new PlaylistCreatedResponse
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description,
            Tracks = new List<PlaylistTrackResponse>()
        };
    }

    public async Task<IEnumerable<PlaylistResponse>> GetPlaylistsAsync(
        ulong userId)
    {
        if (userId == 0)
        {
            throw new ArgumentException(
                "A valid user ID is required.",
                nameof(userId));
        }

        return await _dbContext.Playlists
            .AsNoTracking()
            .Where(playlist => playlist.UserId == userId)
            .OrderBy(playlist => playlist.Id)
            .Select(playlist => new PlaylistResponse
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Description = playlist.Description
            })
            .ToListAsync();
    }
}
