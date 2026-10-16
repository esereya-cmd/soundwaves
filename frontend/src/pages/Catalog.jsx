
import { useEffect, useState } from "react";

import SongCard from "../components/media/SongCard";
import AlbumCard from "../components/media/AlbumCard";
import ArtistCard from "../components/media/ArtistCard";

import { mockAlbums, mockArtists } from "../data/mockCatalog";

import "../styles/catalog.css";

function Catalog({ onSelectSong, onSelectAlbum, onSelectArtist }) {
  const [tracks, setTracks] = useState([]);
  const [albums, setAlbums] = useState(mockAlbums);
  const [artists, setArtists] = useState(mockArtists);

  const [searchQuery, setSearchQuery] = useState("");
  const [hasSearched, setHasSearched] = useState(false);

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
    event.preventDefault();

    const query = searchQuery.trim();

    if (query.length === 1) {
      setError("Please enter at least 2 characters.");
      return;
    }

    setLoading(true);
    setError("");

    try {
      if (query === "") {
        const response = await fetch("/api/tracks");

        if (!response.ok) {
          throw new Error("Failed to load tracks");
        }

        const data = await response.json();

        setTracks(data);
        setAlbums(mockAlbums);
        setArtists(mockArtists);
        setHasSearched(false);
      } else {
        const response = await fetch(
          `/api/catalog/search?query=${encodeURIComponent(query)}`
        );

        if (!response.ok) {
          throw new Error("Failed to search music");
        }

        const data = await response.json();

        setTracks(data.tracks ?? []);
        setAlbums(data.albums ?? []);
        setArtists(data.artists ?? []);
        setHasSearched(true);
      }
    } catch (err) {
      console.error("Error searching music:", err);
      setError("Unable to search music. Please try again later.");
    } finally {
      setLoading(false);
    }
  }

  const noResults =
    hasSearched &&
    tracks.length === 0 &&
    albums.length === 0 &&
    artists.length === 0;

  return (
    <main className="catalog-page">
      <h1>Music Catalog</h1>

      <section className="catalog-section">
        <h2>Search Music</h2>

        <form onSubmit={handleSearch}>
          <input
            type="text"
            placeholder="Search tracks, albums, or artists"
            aria-label="Search music"
            value={searchQuery}
            onChange={(event) => setSearchQuery(event.target.value)}
          />

          <button type="submit" disabled={loading}>
            {loading ? "Loading..." : "Search"}
          </button>
        </form>
      </section>

      {error && <p role="alert">{error}</p>}

      {loading && <p>Loading music...</p>}

      {!loading && !error && noResults && (
        <p>No results found. Try another search.</p>
      )}

      {!loading && !error && !noResults && (
        <>
          <section className="catalog-section">
            <h2>Songs</h2>

            {tracks.length === 0 ? (
              <p>No songs found.</p>
            ) : (
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

            {albums.length === 0 ? (
              <p>No albums found.</p>
            ) : (
              <div className="catalog-grid">
                {albums.map((album) => (
                  <AlbumCard
                    key={album.id}
                    title={album.title}
                    artist={album.artist}
                    onSelect={() => onSelectAlbum(album)}
                  />
                ))}
              </div>
            )}
          </section>

          <section className="catalog-section">
            <h2>Artists</h2>

            {artists.length === 0 ? (
              <p>No artists found.</p>
            ) : (
              <div className="catalog-grid">
                {artists.map((artist) => (
                  <ArtistCard
                    key={artist.id}
                    name={artist.name}
                    onSelect={() => onSelectArtist(artist)}
                  />
                ))}
              </div>
            )}
          </section>
        </>
      )}
    </main>
  );
}

export default Catalog;
