# SCRUM-123: Soundwaves demonstration audio assets

These are two original synthetic test tones generated with FFmpeg, not commercial music. They are intended solely as reproducible MP3 playback fixtures for the Soundwaves development demo.

| File | Frequency | Approx. duration | Format |
| --- | --- | --- | --- |
| `media/demo-tone-a.mp3` | 440 Hz | 35 seconds | MP3, 128 kb/s |
| `media/demo-tone-b.mp3` | 660 Hz | 45 seconds | MP3, 128 kb/s |

## Distribution

Extract the supplied ZIP into a shared project-local directory (location to be finalized in SCRUM-124). Keep the relative `media/` filenames unchanged. Alternatively, teammates can regenerate the exact type of fixture with FFmpeg:

```powershell
New-Item -ItemType Directory -Force media
ffmpeg -f lavfi -i "sine=frequency=440:duration=35" -c:a libmp3lame -b:a 128k -y media/demo-tone-a.mp3
ffmpeg -f lavfi -i "sine=frequency=660:duration=45" -c:a libmp3lame -b:a 128k -y media/demo-tone-b.mp3
```

## Verification

```powershell
ffprobe -v error -show_entries format=duration,size -of default=noprint_wrappers=1 media/demo-tone-a.mp3
ffprobe -v error -show_entries format=duration,size -of default=noprint_wrappers=1 media/demo-tone-b.mp3
```

Play both files locally and confirm audible sound. The generated files were successfully probed as 35.03 s and 45.04 s in the preparation environment. Browser playback and team-machine availability still need manual confirmation.

## Usage and scope

These synthetic tones were generated specifically for testing and contain no sampled commercial music. No external music licensing or download credentials are needed. They are not meant to represent production music. SCRUM-124 decides the canonical storage path; SCRUM-125 will connect database track IDs to these files.
