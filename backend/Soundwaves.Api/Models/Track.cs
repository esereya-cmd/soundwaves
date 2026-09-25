namespace Soundwaves.Api.Models;

public class Track
{
public ulong Id { get; set; }
public ulong? AlbumId { get; set; }
public ulong? ArtistId { get; set; }
public string Title { get; set; } = string.Empty;
public uint? DurationMs { get; set; }
public ushort? TrackNumber { get; set; }
public string FilePath { get; set; } = string.Empty;
public string? MimeType { get; set; }
public ulong? FileSizeBytes { get; set; }
public DateTime CreatedAt { get; set; }
}