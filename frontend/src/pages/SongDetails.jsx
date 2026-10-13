
import { useEffect, useState } from "react";
import "../styles/songDetails.css";

function SongDetails({ songId, onBack }) {
  const [song, setSong] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    async function loadSongDetails() {
      setLoading(true);
      setError("");
      setSong(null);

      if (songId === null || songId === undefined) {
        setError("No song selected.");
        setLoading(false);
        return;
      }

      try {
        const response = await fetch(
          `/api/tracks/${encodeURIComponent(songId)}`
        );

        if (response.status === 404) {
          throw new Error("Song not found.");
        }

        if (!response.ok) {
          throw new Error("Unable to load song details.");
        }

        const data = await response.json();

        if (active) {
          setSong(data);
        }
      } catch (err) {
        console.error("Error loading song details:", err);

        if (active) {
          setError(err.message || "Unable to load song details.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    loadSongDetails();

    return () => {
      active = false;
    };
  }, [songId]);

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

      {loading && <p>Loading song details...</p>}

      {!loading && error && (
        <p role="alert">{error}</p>
      )}

      {!loading && !error && song && (
        <section className="song-details-card">
          <h2>{song.title}</h2>

          <p>
            Artist:{" "}
            {song.artist?.name ??
              song.artistName ??
              (song.artistId != null
                ? `Artist ID: ${song.artistId}`
                : "Unknown")}
          </p>

          <p>
            Album:{" "}
            {song.album?.title ??
              song.albumTitle ??
              (song.albumId != null
                ? `Album ID: ${song.albumId}`
                : "Unknown")}
          </p>
        </section>
      )}
    </main>
  );
}

export default SongDetails;
