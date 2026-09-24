import SongCard from "../components/media/SongCard";
import { mockSongs } from "../data/mockCatalog";

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
      </section>

      <section>
        <h2>Artists</h2>
      </section>
    </main>
  );
}

export default Catalog;