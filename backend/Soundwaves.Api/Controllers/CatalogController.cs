
using Microsoft.AspNetCore.Mvc;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly IMusicCatalogService _musicCatalogService;

    public CatalogController(IMusicCatalogService musicCatalogService)
    {
        _musicCatalogService = musicCatalogService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("A search query is required.");
        }

        query = query.Trim();

        if (query.Length < 2)
        {
            return BadRequest(
                "Search query must be at least 2 characters long.");
        }

        var results = await _musicCatalogService
            .SearchCatalogAsync(query);

        return Ok(results);
    }
}
