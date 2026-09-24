import SongCard from "../components/media/SongCard";
import AlbumCard from "../components/media/AlbumCard";
import ArtistCard from "../components/media/ArtistCard";
import {
  mockSongs,
  mockAlbums,
  mockArtists,
} from "../data/mockCatalog";

function Catalog() {
  return (
    <main>
      <h1>Music Catalog</h1>

      <section>
        <h2>Songs</h2>

        {mockSongs.map((song) => (
          <SongCard
            key={song.id}
            title={song.title}
            artist={song.artist}
            album={song.album}
          />
        ))}
      </section>

      <section>
        <h2>Albums</h2>

        {mockAlbums.map((album) => (
          <AlbumCard
            key={album.id}
            title={album.title}
            artist={album.artist}
          />
        ))}
      </section>

      <section>
        <h2>Artists</h2>

        {mockArtists.map((artist) => (
          <ArtistCard
            key={artist.id}
            name={artist.name}
          />
        ))}
      </section>
    </main>
  );
}

export default Catalog;