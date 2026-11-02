import { useRef, useState } from "react";
import "../../styles/musicPlayer.css";

function MusicPlayer({ selectedTrack }) {
  const audioRef = useRef(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [error, setError] = useState("");

  if (!selectedTrack) {
    return (
      <section className="music-player">
        <h2>Music Player</h2>
        <p>No track selected.</p>
      </section>
    );
  }

  async function handlePlayPause() {
    const audio = audioRef.current;

    if (!audio) {
      return;
    }

    setError("");

    try {
      if (isPlaying) {
        audio.pause();
        setIsPlaying(false);
      } else {
        await audio.play();
        setIsPlaying(true);
      }
    } catch (err) {
      console.error("Playback error:", err);
      setError("Unable to play this track.");
      setIsPlaying(false);
    }
  }

  return (
    <section className="music-player">
      <h2>Music Player</h2>

      <p>
        Now Selected: <strong>{selectedTrack.title}</strong>
      </p>

      <audio
        ref={audioRef}
        src={`/api/tracks/${selectedTrack.id}/stream`}
        onEnded={() => setIsPlaying(false)}
      />

      <button type="button" onClick={handlePlayPause}>
        {isPlaying ? "Pause" : "Play"}
      </button>

      <p>Status: {isPlaying ? "Playing" : "Paused"}</p>

      {error && <p role="alert">{error}</p>}
    </section>
  );
}

export default MusicPlayer;