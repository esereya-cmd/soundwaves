
using Soundwaves.Api.DTOs;

namespace Soundwaves.Api.Services;

public interface IPlaylistService
{
    // Create a playlist belonging to the specified user.
    Task<PlaylistCreatedResponse> CreatePlaylistAsync(
        ulong userId,
        CreatePlaylistRequest request);

    // Retrieve playlists belonging to the specified user.
    Task<IEnumerable<PlaylistResponse>> GetPlaylistsAsync(
        ulong userId);
}
