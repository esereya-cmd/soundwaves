import { useEffect, useRef, useState } from "react";
import "../../styles/musicPlayer.css";

function PlayerControls({
  selectedTrack,
  volume,
  isMuted,
  onVolumeChange,
  onMuteToggle,
}) {
  const audioRef = useRef(null);
  const [isPlaying, setIsPlaying] = useState(false);
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

