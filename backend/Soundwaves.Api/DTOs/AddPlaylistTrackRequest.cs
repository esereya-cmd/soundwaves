
using System.ComponentModel.DataAnnotations;

namespace Soundwaves.Api.DTOs;

public class AddPlaylistTrackRequest
{
    [Required(ErrorMessage = "Track ID is required.")]
    [Range(
        typeof(ulong),
        "1",
        "18446744073709551615",
        ErrorMessage = "Track ID must be greater than zero.")]
    public ulong? TrackId { get; set; }
}
