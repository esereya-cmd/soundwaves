import "../styles/songDetails.css";

function SongDetails({ song, onBack }) {
  if (!song) {
    return <p>No song selected.</p>;
  }

  return (
    <main className="song-details">
      <button
        type="button"
        className="back-button"
        onClick={onBack}
      >
        Back to Catalog
      </button>

      <h1>Song Details</h1>

      <section className="song-details-card">
        <h2>{song.title}</h2>
        <p>Artist: {song.artist}</p>
        <p>Album: {song.album}</p>
      </section>
    </main>
  );
}

export default SongDetails;