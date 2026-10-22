
import { useEffect, useState } from "react";

function ArtistDetails({ artistId, onBack }) {
  const [artist, setArtist] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    async function loadArtist() {
      setLoading(true);
      setError("");
      setArtist(null);

      if (artistId === null || artistId === undefined || String(artistId) === "0") {
        setError("Invalid artist ID.");
        setLoading(false);
        return;
      }

      try {
        const response = await fetch(
          `/api/artists/${encodeURIComponent(artistId)}`
        );

        if (response.status === 404) {
          throw new Error("Artist not found.");
        }

        if (!response.ok) {
          throw new Error("Unable to load artist details.");
        }

        const data = await response.json();

        if (active) {
          setArtist(data);
        }
      } catch (err) {
        if (active) {
          setError(err.message || "Unable to load artist details.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    loadArtist();

    return () => {
      active = false;
    };
  }, [artistId]);

  return (
    <main className="artist-details">
      <button type="button" onClick={onBack}>
        Back to Catalog
      </button>

      <h1>Artist Details</h1>

      {loading && <p>Loading artist details...</p>}

      {!loading && error && <p role="alert">{error}</p>}

      {!loading && !error && artist && (
        <section className="artist-details-card">
          <h2>{artist.name}</h2>

          <h3>Albums</h3>

          {Array.isArray(artist.albums) && artist.albums.length > 0 ? (
            <ul>
              {artist.albums.map((album) => (
                <li key={album.id}>{album.title}</li>
              ))}
            </ul>
          ) : (
            <p>No albums available.</p>
          )}

          <h3>Tracks</h3>

          {Array.isArray(artist.tracks) && artist.tracks.length > 0 ? (
            <ul>
              {artist.tracks.map((track) => (
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

export default ArtistDetails;
