
using System.ComponentModel.DataAnnotations;

namespace Soundwaves.Api.DTOs;

public class UpdatePlaylistRequest
{
    [Required(ErrorMessage = "Playlist name is required.")]
    [StringLength(255, ErrorMessage = "Playlist name cannot exceed 255 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = true,
        ErrorMessage = "Playlist description field is required.")]
    [StringLength(1000,
        ErrorMessage = "Playlist description cannot exceed 1000 characters.")]
    public string? Description { get; set; }
}
