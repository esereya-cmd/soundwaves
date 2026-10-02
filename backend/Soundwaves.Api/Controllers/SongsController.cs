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
        
        if (query.Trim().Length < 2)
        {
            return BadRequest("Search query must be at least 2 characters long.");
        }

        var results = _musicCatalogService.SearchTracks(query.Trim());

        return Ok(results);
    }
}