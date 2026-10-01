using Microsoft.AspNetCore.Mvc;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Controllers;

[ApiController]
[Route("api/songs")]
public class SongsController : ControllerBase
{
    private readonly IMusicCatalogService _musicCatalogService;

    public SongsController(IMusicCatalogService musicCatalogService)
    {
        _musicCatalogService = musicCatalogService;
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("A search query is required.");
        }

        var results = _musicCatalogService.SearchTracks(query);

        return Ok(results);
    }
}