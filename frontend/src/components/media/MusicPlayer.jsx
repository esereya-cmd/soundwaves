import "../../styles/musicPlayer.css";
function MusicPlayer({ selectedTrack }) {
  if (!selectedTrack) {
    return (
      <section className="music-player">
        <h2>Music Player</h2>
        <p>No track selected.</p>
      </section>
    );
  }

  return (
    <section className="music-player">
      <h2>Music Player</h2>

      <p>
        Now Selected: <strong>{selectedTrack.title}</strong>
      </p>

      <p>Status: Ready</p>
    </section>
  );
}

export default MusicPlayer;