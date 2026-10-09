
using System.Collections.Generic;

namespace Soundwaves.Api.DTOs;

public class ArtistDetailsResponse
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<AlbumSummaryResponse> Albums { get; set; } = new();
    public List<TrackSummaryResponse> Tracks { get; set; } = new();
}

public class AlbumSummaryResponse
{
    public ulong Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ArtworkPath { get; set; }
}
