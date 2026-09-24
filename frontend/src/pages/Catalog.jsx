import SongCard from "../components/media/SongCard";
import AlbumCard from "../components/media/AlbumCard";
import ArtistCard from "../components/media/ArtistCard";

import {
  mockSongs,
  mockAlbums,
  mockArtists,
} from "../data/mockCatalog";

import "../styles/catalog.css";

function Catalog() {
  return (
    <main className="catalog-page">
      <h1>Music Catalog</h1>

      <section className="catalog-section">
        <h2>Songs</h2>

        <div className="catalog-grid">
          {mockSongs.map((song) => (
            <SongCard
              key={song.id}
              title={song.title}
              artist={song.artist}
              album={song.album}
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