
using System;
using System.Collections.Generic;

namespace Soundwaves.Api.DTOs;

public class AlbumDetailsResponse
{
    public ulong Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly? ReleaseDate { get; set; }
    public string? ArtworkPath { get; set; }

    public ArtistSummaryResponse? Artist { get; set; }

    public List<TrackSummaryResponse> Tracks { get; set; } = new();
}

public class ArtistSummaryResponse
{
    public ulong Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class TrackSummaryResponse
{
    public ulong Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public uint? DurationMs { get; set; }
    public ushort? TrackNumber { get; set; }
}
