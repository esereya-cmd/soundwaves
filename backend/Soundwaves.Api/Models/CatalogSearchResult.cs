namespace Soundwaves.Api.Models;

public class CatalogSearchResult
{
    public IEnumerable<Track> Tracks { get; set; } = [];
    public IEnumerable<Album> Albums { get; set; } = [];
    public IEnumerable<Artist> Artists { get; set; } = [];
}