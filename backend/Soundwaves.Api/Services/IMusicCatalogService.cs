using Soundwaves.Api.Models;

namespace Soundwaves.Api.Services;

public interface IMusicCatalogService
{
    // Existing track operations
    IEnumerable<Track> GetTracks();
    Track? GetTrackById(ulong id);

    // Existing search operation
    IEnumerable<Track> SearchTracks(string query);

    // Asynchronous album operations
    Task<IEnumerable<Album>> GetAlbumsAsync();
    Task<Album?> GetAlbumByIdAsync(ulong id);

    // Asynchronous artist operations
    Task<IEnumerable<Artist>> GetArtistsAsync();
    Task<Artist?> GetArtistByIdAsync(ulong id);

    // Asynchronous relationship operations
    Task<IEnumerable<Album>> GetAlbumsByArtistIdAsync(ulong artistId);
    Task<IEnumerable<Track>> GetTracksByAlbumIdAsync(ulong albumId);
    Task<IEnumerable<Track>> GetTracksByArtistIdAsync(ulong artistId);
}