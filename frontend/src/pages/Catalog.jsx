
import { useEffect, useState } from "react";

import SongCard from "../components/media/SongCard";
import AlbumCard from "../components/media/AlbumCard";
import ArtistCard from "../components/media/ArtistCard";

import "../styles/catalog.css";

function Catalog({ onSelectSong, onSelectAlbum }) {
  const [tracks, setTracks] = useState([]);
  const [albums, setAlbums] = useState([]);
  const [artists, setArtists] = useState([]);

  const [searchQuery, setSearchQuery] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;

    async function loadCatalog() {
      try {
        const [tracksResponse, albumsResponse, artistsResponse] =
          await Promise.all([
            fetch("/api/tracks"),
            fetch("/api/albums"),
            fetch("/api/artists")
          ]);

        if (
          !tracksResponse.ok ||
          !albumsResponse.ok ||
          !artistsResponse.ok
        ) {
          throw new Error("Failed to load music catalog");
        }

        const tracksData = await tracksResponse.json();
        const albumsData = await albumsResponse.json();
        const artistsData = await artistsResponse.json();

        if (active) {
          setTracks(tracksData);
          setAlbums(albumsData);
          setArtists(artistsData);
          setError("");
        }
      } catch (err) {
        console.error("Error loading catalog:", err);

        if (active) {
          setError("Unable to load music catalog. Please try again later.");
        }
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    }

    loadCatalog();

    return () => {
      active = false;
    };
  }, []);

  async function handleSearch(event) {
    event.preventDefault();

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

        {!loading && albums.length === 0 && (
          <p>No albums available.</p>
        )}

        <div className="catalog-grid">
          {albums.map((album) => {
            const artist = artists.find(
              (artist) => artist.id === album.artistId
            );

            return (
              <AlbumCard
                key={album.id}
                title={album.title}
                artist={artist?.name ?? "Unknown Artist"}
                onSelect={() => onSelectAlbum(album)}
              />
            );
          })}
        </div>
      </section>

      <section className="catalog-section">
        <h2>Artists</h2>

        {!loading && artists.length === 0 && (
          <p>No artists available.</p>
        )}

        <div className="catalog-grid">
          {artists.map((artist) => (
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
