
import { useEffect, useRef, useState } from "react";
import "../../styles/musicPlayer.css";

function formatTime(seconds) {
  if (!Number.isFinite(seconds) || seconds < 0) {
    return "0:00";
  }

  const totalSeconds = Math.floor(seconds);
  const minutes = Math.floor(totalSeconds / 60);
  const remainingSeconds = totalSeconds % 60;

  return `${minutes}:${String(remainingSeconds).padStart(2, "0")}`;
}

function PlayerControls({
  selectedTrack,
  volume,
  isMuted,
  onVolumeChange,
  onMuteToggle,
}) {
  const audioRef = useRef(null);

  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [error, setError] = useState("");

  useEffect(() => {
    const audio = audioRef.current;

    if (audio) {
      audio.volume = volume;
      audio.muted = isMuted;
    }
  }, [volume, isMuted]);

  async function handlePlayPause() {
    const audio = audioRef.current;

    if (!audio) {
      return;
    }

    setError("");

    try {
      if (audio.paused) {
        await audio.play();
      } else {
        audio.pause();
      }
    } catch (err) {
      console.error("Playback error:", err);
      setError("Unable to play this track.");
      setIsPlaying(false);
    }
  }

  function handleTimeUpdate() {
    const audio = audioRef.current;

    if (audio) {
      setCurrentTime(audio.currentTime);
    }
  }

  function handleDurationChange() {
    const audio = audioRef.current;

    if (audio && Number.isFinite(audio.duration) && audio.duration > 0) {
      setDuration(audio.duration);
    } else {
      setDuration(0);
    }
  }

  function handleSeek(event) {
    const audio = audioRef.current;

    if (!audio || !Number.isFinite(audio.duration) || audio.duration <= 0) {
      return;
    }

    const newTime = Number(event.target.value);

    audio.currentTime = newTime;
    setCurrentTime(newTime);
  }

  return (
    <>
      <p>
        Now Selected: <strong>{selectedTrack.title}</strong>
      </p>

      <audio
        ref={audioRef}
        src={`/api/tracks/${encodeURIComponent(selectedTrack.id)}/stream`}
        onPlay={() => setIsPlaying(true)}
        onPause={() => setIsPlaying(false)}
        onEnded={() => setIsPlaying(false)}
        onTimeUpdate={handleTimeUpdate}
        onLoadedMetadata={handleDurationChange}
        onDurationChange={handleDurationChange}
        onError={() => {
          setError("Unable to play this track.");
          setIsPlaying(false);
        }}
      />

      <button type="button" onClick={handlePlayPause}>
        {isPlaying ? "Pause" : "Play"}
      </button>

      <p>Status: {isPlaying ? "Playing" : "Paused"}</p>

      <div>
        <label htmlFor="seek">Seek: </label>

        {duration > 0 ? (
          <input
            key="seek-ready"
            id="seek"
            type="range"
            min="0"
            max={duration}
            step="0.1"
            value={Math.min(currentTime, duration)}
            onChange={handleSeek}
          />
        ) : (
          <input
            key="seek-unavailable"
            id="seek"
            type="range"
            min="0"
            max="1"
            step="0.1"
            value="0"
            disabled
          />
        )}

        <p>
          {formatTime(currentTime)} / {formatTime(duration)}
        </p>
      </div>

      <div>
        <label htmlFor="volume">Volume: </label>

        <input
          id="volume"
          type="range"
          min="0"
          max="1"
          step="0.1"
          value={volume}
          onChange={onVolumeChange}
        />

        <button type="button" onClick={onMuteToggle}>
          {isMuted ? "Unmute" : "Mute"}
        </button>
      </div>

      {error && <p role="alert">{error}</p>}
    </>
  );
}

function MusicPlayer({ selectedTrack }) {
  const [volume, setVolume] = useState(1);
  const [isMuted, setIsMuted] = useState(false);

  function handleVolumeChange(event) {
    const newVolume = Number(event.target.value);

    setVolume(newVolume);
    setIsMuted(false);
  }

  function handleMuteToggle() {
    setIsMuted(!isMuted);
  }

  return (
    <section className="music-player">
      <h2>Music Player</h2>

      {!selectedTrack ? (
        <p>No track selected.</p>
      ) : (
        <PlayerControls
          key={selectedTrack.id}
          selectedTrack={selectedTrack}
          volume={volume}
          isMuted={isMuted}
          onVolumeChange={handleVolumeChange}
          onMuteToggle={handleMuteToggle}
        />
      )}
    </section>
  );
}

export default MusicPlayer;
