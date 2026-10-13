import { useEffect, useState } from "react";

function AlbumDetails({ albumId, onBack }) {
  const [album, setAlbum] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    async function loadAlbum() {
      setLoading(true);
      setError("");
      setAlbum(null);

      if (albumId === null || albumId === undefined) {
        setError("No album selected.");
        setLoading(false);
        return;
      }

      try {
        const response = await fetch(
          `/api/albums/${encodeURIComponent(albumId)}`
        );

        if (response.status === 404) {
          throw new Error("Album not found.");
        }

        if (!response.ok) {
          throw new Error("Unable to load album details.");
        }

        const data = await response.json();

        if (active) {
          setAlbum(data);
        }
      } catch (err) {
        if (active) {
          setError(err.message || "Unable to load album details.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    loadAlbum();

    return () => {
      active = false;
    };
  }, [albumId]);

  return (
    <main className="album-details">
      <button type="button" onClick={onBack}>
        Back to Catalog
      </button>

      <h1>Album Details</h1>

      {loading && <p>Loading album details...</p>}

      {!loading && error && <p role="alert">{error}</p>}

      {!loading && !error && album && (
        <section className="album-details-card">
          <h2>{album.title}</h2>

          <p>
            Artist: {album.artist?.name ?? album.artistName ?? "Unknown"}
          </p>

          <h3>Tracks</h3>

          {Array.isArray(album.tracks) && album.tracks.length > 0 ? (
            <ul>
              {album.tracks.map((track) => (
                <li key={track.id}>{track.title}</li>
              ))}
            </ul>
          ) : (
            <p>No tracks available.</p>
          )}
        </section>
      )}
    </main>
  );
}

export default AlbumDetails;