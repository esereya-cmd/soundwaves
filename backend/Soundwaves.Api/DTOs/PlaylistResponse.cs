
namespace Soundwaves.Api.DTOs;

public class PlaylistResponse
{
    public ulong Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
