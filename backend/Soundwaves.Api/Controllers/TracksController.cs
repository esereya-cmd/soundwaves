
using Microsoft.AspNetCore.Mvc;
using Soundwaves.Api.Models;
using Soundwaves.Api.Services;

namespace Soundwaves.Api.Controllers;

[ApiController]
[Route("api/tracks")]
public class TracksController : ControllerBase
{
    private readonly IMusicCatalogService _musicCatalogService;
    private readonly IAudioFileLookupService _audioFileLookupService;

    public TracksController(
        IMusicCatalogService musicCatalogService,
        IAudioFileLookupService audioFileLookupService)
    {
        _musicCatalogService = musicCatalogService;
        _audioFileLookupService = audioFileLookupService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Track>>> GetTracks()
    {
        var tracks = await _musicCatalogService.GetTracksAsync();

        return Ok(tracks);
    }

    [HttpGet("{id}")]
    public ActionResult<Track> GetTrackById(ulong id)
    {
        if (id == 0)
        {
            return BadRequest("Track ID must be greater than 0.");
        }

        var track = _musicCatalogService.GetTrackById(id);

        if (track == null)
        {
            return NotFound();
        }

        return Ok(track);
    }

    // SCRUM-132: Stream MP3 audio for an existing track.
    // SCRUM-133: Enable HTTP byte-range processing for seeking.
    [HttpGet("{id}/stream")]
    public async Task<IActionResult> StreamTrack(ulong id)
    {
        if (id == 0)
        {
            return BadRequest("Track ID must be greater than 0.");
        }

        var audioFilePath =
            await _audioFileLookupService.GetAudioFilePathAsync(id);

        if (audioFilePath == null)
        {
            return NotFound("Audio file not found.");
        }

        return PhysicalFile(
            audioFilePath,
            "audio/mpeg",
            enableRangeProcessing: true);
    }
}
