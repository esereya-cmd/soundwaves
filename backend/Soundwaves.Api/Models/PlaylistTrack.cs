
namespace Soundwaves.Api.Models;

public class PlaylistTrack
{
    public ulong PlaylistId { get; set; }

    public ulong TrackId { get; set; }

    public uint Position { get; set; }

    public DateTime AddedAt { get; set; }
}
