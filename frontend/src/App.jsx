
import { useState } from "react";

import Catalog from "./pages/Catalog";
import SongDetails from "./pages/SongDetails";
import AlbumDetails from "./pages/AlbumDetails";
import ArtistDetails from "./pages/ArtistDetails";

function App() {
  const [selectedSong, setSelectedSong] = useState(null);
  const [selectedAlbum, setSelectedAlbum] = useState(null);
  const [selectedArtist, setSelectedArtist] = useState(null);

  if (selectedArtist) {
    return (
      <ArtistDetails
        artistId={selectedArtist.id}
        onBack={() => setSelectedArtist(null)}
      />
    );
  }

  if (selectedAlbum) {
    return (
      <AlbumDetails
        albumId={selectedAlbum.id}
        onBack={() => setSelectedAlbum(null)}
      />
    );
  }

  if (selectedSong) {
    return (
      <SongDetails
        songId={selectedSong.id}
        onBack={() => setSelectedSong(null)}
      />
    );
  }

  return (
    <Catalog
      onSelectSong={setSelectedSong}
      onSelectAlbum={setSelectedAlbum}
      onSelectArtist={setSelectedArtist}
    />
  );
}

export default App;
