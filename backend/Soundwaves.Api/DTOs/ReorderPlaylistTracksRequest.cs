
using System.ComponentModel.DataAnnotations;

namespace Soundwaves.Api.DTOs;

public class ReorderPlaylistTracksRequest
{
    [Required(ErrorMessage = "Track IDs are required.")]
    public List<ulong>? TrackIds { get; set; }
}

