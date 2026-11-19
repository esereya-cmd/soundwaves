
namespace Soundwaves.Api.DTOs;

public class PlaylistDetailsResponse
{
    public ulong Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<PlaylistTrackResponse> Tracks { get; set; } = new();
}
