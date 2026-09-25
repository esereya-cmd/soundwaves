namespace Soundwaves.Api.Models;

public class Artist
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}