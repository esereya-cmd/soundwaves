
using Soundwaves.Api.DTOs;

namespace Soundwaves.Api.Services;

public interface IPlaylistService
{
    // Create a playlist belonging to the specified user.
    Task<PlaylistCreatedResponse> CreatePlaylistAsync(
        ulong userId,
        CreatePlaylistRequest request);

    // Retrieve all playlists belonging to the specified user.
    Task<IEnumerable<PlaylistResponse>> GetPlaylistsAsync(
        ulong userId);

    // Retrieve a specific playlist and its ordered tracks.
    Task<PlaylistDetailsResponse?> GetPlaylistByIdAsync(
        ulong userId,
        ulong playlistId);

    // Update a playlist's name and description.
    Task<PlaylistResponse?> UpdatePlaylistAsync(
        ulong userId,
        ulong playlistId,
        UpdatePlaylistRequest request);
}
