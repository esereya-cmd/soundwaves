namespace Soundwaves.Api.Models;

public class Album
{
    public ulong Id { get; set; }
    public ulong? ArtistId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly? ReleaseDate { get; set; }
    public string? ArtworkPath { get; set; }
    public DateTime CreatedAt { get; set; }
}