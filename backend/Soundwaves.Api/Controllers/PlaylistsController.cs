
using Microsoft.AspNetCore.Mvc;
using Soundwaves.Api.DTOs;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Controllers;

[ApiController]
[Route("api/playlists")]
public class PlaylistsController : ControllerBase
{
    private readonly IPlaylistService _playlistService;
    private readonly ICurrentUserService _currentUserService;

    public PlaylistsController(
        IPlaylistService playlistService,
        ICurrentUserService currentUserService)
    {
        _playlistService = playlistService;
        _currentUserService = currentUserService;
    }

    // POST /api/playlists
    [HttpPost]
    public async Task<IActionResult> CreatePlaylist(
        [FromBody] CreatePlaylistRequest request)
    {
        ulong? userId = _currentUserService.GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "A valid user identity is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "Playlist name cannot be empty."
            });
        }

        try
        {
            var playlist = await _playlistService.CreatePlaylistAsync(
                userId.Value,
                request);

            return Created(
                $"/api/playlists/{playlist.Id}",
                playlist);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = ex.Message
            });
        }
    }

    // GET /api/playlists
    [HttpGet]
    public async Task<IActionResult> GetPlaylists()
    {
        ulong? userId = _currentUserService.GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "A valid user identity is required."
            });
        }

        var playlists = await _playlistService.GetPlaylistsAsync(
            userId.Value);

        return Ok(playlists);
    }

    // GET /api/playlists/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlaylistById(ulong id)
    {
        ulong? userId = _currentUserService.GetCurrentUserId();

        // Require a valid server-side identity.
        if (!userId.HasValue)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "A valid user identity is required."
            });
        }

        // Reject invalid playlist identifiers.
        if (id == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "Playlist ID must be greater than zero."
            });
        }

        // Retrieve the playlist only if it belongs to the user.
        var playlist = await _playlistService.GetPlaylistByIdAsync(
            userId.Value,
            id);

        if (playlist == null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = "Playlist not found."
            });
        }

        return Ok(playlist);
    }


    // PUT /api/playlists/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlaylist(
        ulong id,
        [FromBody] UpdatePlaylistRequest request)
    {
        // Require a valid server-side identity.
        ulong? userId = _currentUserService.GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "A valid user identity is required."
            });
        }

        // Reject invalid playlist identifiers.
        if (id == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "Playlist ID must be greater than zero."
            });
        }

        // Reject empty or whitespace-only playlist names.
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "Playlist name cannot be empty."
            });
        }

        try
        {
            // Update only a playlist owned by the current user.
            var playlist = await _playlistService.UpdatePlaylistAsync(
                userId.Value,
                id,
                request);

            if (playlist == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = "Playlist not found."
                });
            }

            return Ok(playlist);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = ex.Message
            });
        }
    }

    // DELETE /api/playlists/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlaylist(ulong id)
    {
        // Require a valid server-side identity.
        ulong? userId = _currentUserService.GetCurrentUserId();

        if (!userId.HasValue)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "A valid user identity is required."
            });
        }

        // Reject invalid playlist identifiers.
        if (id == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = "Playlist ID must be greater than zero."
            });
        }

        // Delete only a playlist owned by the current user.
        bool deleted = await _playlistService.DeletePlaylistAsync(
            userId.Value,
            id);

        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = "Playlist not found."
            });
        }

        // Successful deletion returns no response body.
        return NoContent();
    }
}