import { useState } from "react";

import Catalog from "./pages/Catalog";
import SongDetails from "./pages/SongDetails";
import AlbumDetails from "./pages/AlbumDetails";
import ArtistDetails from "./pages/ArtistDetails";
import MusicPlayer from "./components/media/MusicPlayer";

function App() {
  const [selectedSong, setSelectedSong] = useState(null);
  const [selectedAlbum, setSelectedAlbum] = useState(null);
  const [selectedArtist, setSelectedArtist] = useState(null);

  const [selectedTrack, setSelectedTrack] = useState(null);

  if (selectedArtist) {
    return (
      <>
        <ArtistDetails
          artistId={selectedArtist.id}
          onBack={() => setSelectedArtist(null)}
          onSelectTrack={setSelectedTrack}
        />

        <MusicPlayer selectedTrack={selectedTrack} />
      </>
    );
  }

  if (selectedAlbum) {
    return (
      <>
        <AlbumDetails
          albumId={selectedAlbum.id}
          onBack={() => setSelectedAlbum(null)}
          onSelectTrack={setSelectedTrack}
        />

        <MusicPlayer selectedTrack={selectedTrack} />
      </>
    );
  }

  if (selectedSong) {
    return (
      <>
        <SongDetails
          songId={selectedSong.id}
          onBack={() => setSelectedSong(null)}
        />

        <MusicPlayer selectedTrack={selectedTrack} />
      </>
    );
  }

  return (
    <>
      <Catalog
        onSelectSong={(track) => {
          setSelectedSong(track);
          setSelectedTrack(track);
        }}
        onSelectAlbum={setSelectedAlbum}
        onSelectArtist={setSelectedArtist}
      />

      <MusicPlayer selectedTrack={selectedTrack} />
    </>
  );
}

export default App;
