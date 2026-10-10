
import { useEffect, useState } from "react";

import SongCard from "../components/media/SongCard";
import AlbumCard from "../components/media/AlbumCard";
import ArtistCard from "../components/media/ArtistCard";

import { mockAlbums, mockArtists } from "../data/mockCatalog";

import "../styles/catalog.css";

function Catalog({ onSelectSong }) {
  const [tracks, setTracks] = useState([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    async function loadTracks() {
      try {
        const response = await fetch("/api/tracks");

        if (!response.ok) {
          throw new Error("Failed to load tracks");
        }

        const data = await response.json();

        if (active) {
          setTracks(data);
          setError("");
        }
      } catch (err) {
        console.error("Error loading tracks:", err);

        if (active) {
          setError("Unable to load songs. Please try again later.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    loadTracks();

    return () => {
      active = false;
    };
  }, []);

  async function handleSearch(event) {
    if (event) {
      event.preventDefault();
    }

    setLoading(true);
    setError("");

    const query = searchQuery.trim();

    const url = query
      ? `/api/songs/search?query=${encodeURIComponent(query)}`
      : "/api/tracks";

    try {
      const response = await fetch(url);

      if (!response.ok) {
        throw new Error("Failed to fetch songs");
      }

      const data = await response.json();

      setTracks(data);
    } catch (err) {
      console.error("Error searching tracks:", err);
      setError("Unable to load songs. Please try again later.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="catalog-page">
      <h1>Music Catalog</h1>

      <section className="catalog-section">
        <h2>Search Music</h2>

        <form onSubmit={handleSearch}>
          <input
            type="text"
            placeholder="Search by song or artist"
            aria-label="Search music"
            value={searchQuery}
            onChange={(event) => setSearchQuery(event.target.value)}
          />

          <button type="submit" disabled={loading}>
            {loading ? "Loading..." : "Search"}
          </button>
        </form>
      </section>

      <section className="catalog-section">
        <h2>Songs</h2>

        {loading && <p>Loading songs...</p>}

        {error && <p role="alert">{error}</p>}

        {!loading && !error && tracks.length === 0 && (
          <p>No songs available.</p>
        )}

        {!loading && !error && tracks.length > 0 && (
          <div className="catalog-grid">
            {tracks.map((track) => (
              <SongCard
                key={track.id}
                title={track.title}
                artistId={track.artistId}
                albumId={track.albumId}
                onSelect={() => onSelectSong(track)}
              />
            ))}
          </div>
        )}
      </section>

      <section className="catalog-section">
        <h2>Albums</h2>

        <div className="catalog-grid">
          {mockAlbums.map((album) => (
            <AlbumCard
              key={album.id}
              title={album.title}
              artist={album.artist}
            />
          ))}
        </div>
      </section>

      <section className="catalog-section">
        <h2>Artists</h2>

        <div className="catalog-grid">
          {mockArtists.map((artist) => (
            <ArtistCard
              key={artist.id}
              name={artist.name}
            />
          ))}
        </div>
      </section>
    </main>
  );
}

export default Catalog;
