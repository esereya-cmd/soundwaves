
import { useState } from "react";

import Catalog from "./pages/Catalog";
import SongDetails from "./pages/SongDetails";
import AlbumDetails from "./pages/AlbumDetails";

function App() {
  const [selectedSong, setSelectedSong] = useState(null);
  const [selectedAlbum, setSelectedAlbum] = useState(null);

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
    />
  );
}

export default App;
