import { useEffect, useState } from "react";

import SongCard from "../components/media/SongCard";
import AlbumCard from "../components/media/AlbumCard";
import ArtistCard from "../components/media/ArtistCard";

import {
  mockAlbums,
  mockArtists,
} from "../data/mockCatalog";

import "../styles/catalog.css";

function Catalog({ onSelectSong }) {
  const [tracks, setTracks] = useState([]);
  const [searchQuery, setSearchQuery] = useState("");

  useEffect(() => {
    fetch("/api/tracks")
      .then((response) => response.json())
      .then((data) => setTracks(data))
      .catch((error) => console.error("Error loading tracks:", error));
  }, []);

  function handleSearch() {
    if (searchQuery.trim() === "") {
      fetch("/api/tracks")
        .then((response) => response.json())
        .then((data) => setTracks(data))
        .catch((error) => console.error("Error loading tracks:", error));

      return;
    }

    fetch(`/api/songs/search?query=${encodeURIComponent(searchQuery)}`)
      .then((response) => response.json())
      .then((data) => setTracks(data))
      .catch((error) => console.error("Error searching tracks:", error));
  }

  return (
    <main className="catalog-page">
      <h1>Music Catalog</h1>

      <section className="catalog-section">
        <h2>Search Music</h2>

        <input
          type="text"
          placeholder="Search by song or artist"
          value={searchQuery}
          onChange={(event) => setSearchQuery(event.target.value)}
        />

        <button type="button" onClick={handleSearch}>
          Search
        </button>
      </section>

      <section className="catalog-section">
        <h2>Songs</h2>

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