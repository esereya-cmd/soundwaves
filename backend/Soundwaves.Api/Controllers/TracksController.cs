using Microsoft.AspNetCore.Mvc;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Controllers;

[ApiController]
[Route("api/tracks")]
public class TracksController : ControllerBase
{
    private readonly IMusicCatalogService _musicCatalogService;

    public TracksController(IMusicCatalogService musicCatalogService)
    {
        _musicCatalogService = musicCatalogService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Track>> GetTracks()
    {
        var tracks = _musicCatalogService.GetTracks();

        return Ok(tracks);
    }

     [HttpGet("{id}")]
    public ActionResult<Track> GetTrackById(ulong id)
    {
        var track = _musicCatalogService.GetTrackById(id);

        if (track == null)
        {
            return NotFound();
        }

        return Ok(track);
    }
}

