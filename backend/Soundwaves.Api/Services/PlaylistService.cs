
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

    // Create a playlist for the specified user.
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

    // Retrieve all playlists belonging to the specified user.
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

    // Retrieve a specific playlist and its ordered tracks.
    public async Task<PlaylistDetailsResponse?> GetPlaylistByIdAsync(
        ulong userId,
        ulong playlistId)
    {
        if (userId == 0)
        {
            throw new ArgumentException(
                "A valid user ID is required.",
                nameof(userId));
        }

        if (playlistId == 0)
        {
            throw new ArgumentException(
                "A valid playlist ID is required.",
                nameof(playlistId));
        }

        // Verify that the playlist belongs to the current user.
        var playlist = await _dbContext.Playlists
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.Id == playlistId &&
                p.UserId == userId);

        if (playlist == null)
        {
            return null;
        }

        // Retrieve the playlist's tracks in position order.
        var tracks = await (
            from playlistTrack in _dbContext.PlaylistTracks
            join track in _dbContext.Tracks
                on playlistTrack.TrackId equals track.Id
            where playlistTrack.PlaylistId == playlistId
            orderby playlistTrack.Position
            select new PlaylistTrackResponse
            {
                Id = track.Id,
                Title = track.Title,
                Position = playlistTrack.Position
            }
        )
        .AsNoTracking()
        .ToListAsync();

        return new PlaylistDetailsResponse
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description,
            Tracks = tracks
        };
    }

    // Update a playlist's name and description.
    public async Task<PlaylistResponse?> UpdatePlaylistAsync(
        ulong userId,
        ulong playlistId,
        UpdatePlaylistRequest request)
    {
        // Validate the user ID.
        if (userId == 0)
        {
            throw new ArgumentException(
                "A valid user ID is required.",
                nameof(userId));
        }

        // Validate the playlist ID.
        if (playlistId == 0)
        {
            throw new ArgumentException(
                "A valid playlist ID is required.",
                nameof(playlistId));
        }

        // Validate the request.
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Trim and validate the playlist name.
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

        // Validate the description.
        if (request.Description?.Length > 1000)
        {
            throw new ArgumentException(
                "Playlist description cannot exceed 1000 characters.");
        }

        // Find the playlist and verify ownership.
        var playlist = await _dbContext.Playlists
            .FirstOrDefaultAsync(p =>
                p.Id == playlistId &&
                p.UserId == userId);

        if (playlist == null)
        {
            return null;
        }

        // Update only editable playlist information.
        playlist.Name = name;
        playlist.Description = request.Description;

        // Save changes to the database.
        await _dbContext.SaveChangesAsync();

        // Return the updated playlist.
        return new PlaylistResponse
        {
            Id = playlist.Id,
            Name = playlist.Name,
            Description = playlist.Description
        };
    }

    // Delete a playlist belonging to the specified user.
    public async Task<bool> DeletePlaylistAsync(
        ulong userId,
        ulong playlistId)
    {
        // Validate the user ID.
        if (userId == 0)
        {
            throw new ArgumentException(
                "A valid user ID is required.",
                nameof(userId));
        }

        // Validate the playlist ID.
        if (playlistId == 0)
        {
            throw new ArgumentException(
                "A valid playlist ID is required.",
                nameof(playlistId));
        }

        // Find the playlist and verify ownership.
        var playlist = await _dbContext.Playlists
            .FirstOrDefaultAsync(p =>
                p.Id == playlistId &&
                p.UserId == userId);

        // Do not reveal whether another user's playlist exists.
        if (playlist == null)
        {
            return false;
        }

        // Delete the playlist.
        _dbContext.Playlists.Remove(playlist);

        // Save the deletion to the database.
        await _dbContext.SaveChangesAsync();

        return true;
    }


}
