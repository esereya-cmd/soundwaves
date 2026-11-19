
namespace Soundwaves.Api.DTOs;

public class PlaylistCreatedResponse
{
    public ulong Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<PlaylistTrackResponse> Tracks { get; set; } = new();
}

public class PlaylistTrackResponse
{
    public ulong Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public uint Position { get; set; }
}
