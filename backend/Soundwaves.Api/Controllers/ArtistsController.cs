
using Microsoft.AspNetCore.Mvc;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistsController : ControllerBase
{
    private readonly IMusicCatalogService _musicCatalogService;

    public ArtistsController(IMusicCatalogService musicCatalogService)
    {
        _musicCatalogService = musicCatalogService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistDetailsResponse>> GetArtistById(ulong id)
    {
        if (id == 0)
        {
            return BadRequest("Artist ID must be greater than 0.");
        }

        var artist = await _musicCatalogService.GetArtistByIdAsync(id);

        if (artist == null)
        {
            return NotFound();
        }

        var albums = await _musicCatalogService.GetAlbumsByArtistIdAsync(id);
        var tracks = await _musicCatalogService.GetTracksByArtistIdAsync(id);

        var response = new ArtistDetailsResponse
        {
            Id = artist.Id,
            Name = artist.Name,

            Albums = albums.Select(album => new AlbumSummaryResponse
            {
                Id = album.Id,
                Title = album.Title,
                ArtworkPath = album.ArtworkPath
            }).ToList(),

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
