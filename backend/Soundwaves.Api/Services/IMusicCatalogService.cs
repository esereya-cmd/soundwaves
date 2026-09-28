using Soundwaves.Api.Models;

namespace Soundwaves.Api.Services;

public interface IMusicCatalogService
{
    IEnumerable<Track> GetTracks();
    Track? GetTrackById(ulong id);
    IEnumerable<Artist> GetArtists();
    IEnumerable<Album> GetAlbums();
    IEnumerable<Track> SearchTracks(string query);
}