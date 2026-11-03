namespace Soundwaves.Api.Services;

public interface IAudioFileLookupService
{
    Task<string?> GetAudioFilePathAsync(ulong trackId);
}