
using Microsoft.EntityFrameworkCore;
using Soundwaves.Api.Data;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Models;
using Microsoft.EntityFrameworkCore.Storage;

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

    // Add an existing catalog track to a playlist.
    public async Task<PlaylistTrackResponse?> AddTrackToPlaylistAsync(
        ulong userId,
        ulong playlistId,
        ulong trackId)
    {
        // Validate identifiers.
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

        if (trackId == 0)
        {
            throw new ArgumentException(
                "A valid track ID is required.",
                nameof(trackId));
        }

        // Verify that the playlist belongs to the current user.
        bool playlistExists = await _dbContext.Playlists
            .AnyAsync(p =>
                p.Id == playlistId &&
                p.UserId == userId);

        if (!playlistExists)
        {
            return null;
        }

        // Verify that the track exists in the catalog.
        var track = await _dbContext.Tracks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == trackId);

        if (track == null)
        {
            return null;
        }

        // Prevent duplicate tracks within the same playlist.
        bool duplicateExists = await _dbContext.PlaylistTracks
            .AnyAsync(pt =>
                pt.PlaylistId == playlistId &&
                pt.TrackId == trackId);

        if (duplicateExists)
        {
            throw new InvalidOperationException(
                "This track is already in the playlist.");
        }

        // Determine the next available position.
        uint maxPosition = await _dbContext.PlaylistTracks
            .Where(pt => pt.PlaylistId == playlistId)
            .Select(pt => (uint?)pt.Position)
            .MaxAsync() ?? 0;

        if (maxPosition == uint.MaxValue)
        {
            throw new InvalidOperationException(
                "The playlist has reached its maximum track position.");
        }

        uint nextPosition = maxPosition + 1;

        // Create the playlist-track relationship.
        var playlistTrack = new PlaylistTrack
        {
            PlaylistId = playlistId,
            TrackId = trackId,
            Position = nextPosition,
            AddedAt = DateTime.UtcNow
        };

        _dbContext.PlaylistTracks.Add(playlistTrack);

        await _dbContext.SaveChangesAsync();

        // Return the added track information.
        return new PlaylistTrackResponse
        {
            Id = track.Id,
            Title = track.Title,
            Position = playlistTrack.Position
        };
    }

    // Remove a track and close gaps in playlist positions.
    public async Task<bool> RemoveTrackFromPlaylistAsync(
        ulong userId,
        ulong playlistId,
        ulong trackId)
    {
        if (userId == 0 || playlistId == 0 || trackId == 0)
        {
            throw new ArgumentException(
                "User, playlist, and track IDs must be greater than zero.");
        }

        bool owned = await _dbContext.Playlists
            .AnyAsync(p => p.Id == playlistId && p.UserId == userId);

        if (!owned)
            return false;

        var existing = await _dbContext.PlaylistTracks
            .AsNoTracking()
            .Where(pt => pt.PlaylistId == playlistId)
            .OrderBy(pt => pt.Position)
            .ToListAsync();

        if (!existing.Any(pt => pt.TrackId == trackId))
            return false;

        var remaining = existing
            .Where(pt => pt.TrackId != trackId)
            .ToList();

        await ReplacePlaylistTrackPositionsAsync(
            playlistId, remaining);

        return true;
    }

    // Reorder all tracks using the exact submitted track IDs.
    public async Task<bool> ReorderPlaylistTracksAsync(
        ulong userId,
        ulong playlistId,
        IReadOnlyList<ulong> trackIds)
    {
        if (userId == 0 || playlistId == 0)
        {
            throw new ArgumentException(
                "User and playlist IDs must be greater than zero.");
        }

        if (trackIds == null)
        {
            throw new ArgumentException(
                "Track IDs are required.");
        }

        bool owned = await _dbContext.Playlists
            .AnyAsync(p => p.Id == playlistId && p.UserId == userId);

        if (!owned)
            return false;

        var existing = await _dbContext.PlaylistTracks
            .AsNoTracking()
            .Where(pt => pt.PlaylistId == playlistId)
            .OrderBy(pt => pt.Position)
            .ToListAsync();

        if (trackIds.Count != existing.Count ||
            trackIds.Distinct().Count() != trackIds.Count ||
            !trackIds.OrderBy(id => id)
                .SequenceEqual(existing.Select(pt => pt.TrackId)
                    .OrderBy(id => id)))
        {
            throw new ArgumentException(
                "Reorder request must contain exactly the existing track IDs.");
        }

        var lookup = existing.ToDictionary(pt => pt.TrackId);

        var reordered = trackIds
            .Select(id => lookup[id])
            .ToList();

        await ReplacePlaylistTrackPositionsAsync(
            playlistId, reordered);

        return true;
    }

    // Replace position records while preserving track associations.
    private async Task ReplacePlaylistTrackPositionsAsync(
        ulong playlistId,
        IReadOnlyList<PlaylistTrack> orderedTracks)
    {
        // Relational databases require an atomic transaction.
        bool relational = _dbContext.Database.IsRelational();

        IDbContextTransaction? transaction = null;

        if (relational)
        {
            transaction = await _dbContext.Database
                .BeginTransactionAsync();
        }

        try
        {
            var current = await _dbContext.PlaylistTracks
                .Where(pt => pt.PlaylistId == playlistId)
                .ToListAsync();

            _dbContext.PlaylistTracks.RemoveRange(current);

            await _dbContext.SaveChangesAsync();

            for (int i = 0; i < orderedTracks.Count; i++)
            {
                var original = orderedTracks[i];

                _dbContext.PlaylistTracks.Add(new PlaylistTrack
                {
                    PlaylistId = playlistId,
                    TrackId = original.TrackId,
                    Position = checked((uint)(i + 1)),
                    AddedAt = original.AddedAt
                });
            }

            await _dbContext.SaveChangesAsync();

            if (transaction != null)
            {
                await transaction.CommitAsync();
            }
        }
        catch
        {
            if (transaction != null)
            {
                await transaction.RollbackAsync();
            }

            throw;
        }
        finally
        {
            if (transaction != null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

}
