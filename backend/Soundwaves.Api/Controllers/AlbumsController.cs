
using Microsoft.AspNetCore.Mvc;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Controllers;

[ApiController]
[Route("api/albums")]
public class AlbumsController : ControllerBase
{
    private readonly IMusicCatalogService _musicCatalogService;

    public AlbumsController(IMusicCatalogService musicCatalogService)
    {
        _musicCatalogService = musicCatalogService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AlbumDetailsResponse>> GetAlbumById(ulong id)
    {
        if (id == 0)
        {
            return BadRequest("Album ID must be greater than 0.");
        }

        var album = await _musicCatalogService.GetAlbumByIdAsync(id);

        if (album == null)
        {
            return NotFound();
        }

        var artist = album.ArtistId.HasValue
            ? await _musicCatalogService.GetArtistByIdAsync(album.ArtistId.Value)
            : null;

        var tracks = await _musicCatalogService.GetTracksByAlbumIdAsync(id);

        var response = new AlbumDetailsResponse
        {
            Id = album.Id,
            Title = album.Title,
            ReleaseDate = album.ReleaseDate,
            ArtworkPath = album.ArtworkPath,

            Artist = artist == null
                ? null
                : new ArtistSummaryResponse
                {
                    Id = artist.Id,
                    Name = artist.Name
                },

            Tracks = tracks.Select(track => new TrackSummaryResponse
            {
                Id = track.Id,
                Title = track.Title,
                DurationMs = track.DurationMs,
                TrackNumber = track.TrackNumber
            }).ToList()
        };

        return Ok(response);
    }
}
