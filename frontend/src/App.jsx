
import { useState } from "react";

import Catalog from "./pages/Catalog";
import SongDetails from "./pages/SongDetails";
import AlbumDetails from "./pages/AlbumDetails";
import ArtistDetails from "./pages/ArtistDetails";
import PlaylistList from "./pages/PlaylistList";
import PlaylistDetails from "./pages/PlaylistDetails";
import MusicPlayer from "./components/media/MusicPlayer";

import "./styles/playlist.css";

// Temporary playlist data until the backend API is ready.
const samplePlaylists = [
  {
    id: 1,
    name: "My Favorites",
    tracks: [
      {
        id: 1,
        title: "Sample Track",
      },
    ],
  },
  {
    id: 2,
    name: "Chill Mix",
    tracks: [],
  },
];

function App() {
  const [selectedSong, setSelectedSong] = useState(null);
  const [selectedAlbum, setSelectedAlbum] = useState(null);
  const [selectedArtist, setSelectedArtist] = useState(null);
  const [selectedTrack, setSelectedTrack] = useState(null);

  const [showPlaylists, setShowPlaylists] = useState(false);
  const [selectedPlaylist, setSelectedPlaylist] = useState(null);

  if (showPlaylists) {
    return (
      <>
        {selectedPlaylist ? (
          <PlaylistDetails
            playlist={selectedPlaylist}
            onBack={() => setSelectedPlaylist(null)}
            onSelectTrack={setSelectedTrack}
          />
        ) : (
          <>
            <button
              type="button"
              className="playlist-back-button"
              onClick={() => setShowPlaylists(false)}
            >
              Back to Catalog
            </button>

            <PlaylistList
              playlists={samplePlaylists}
              onSelectPlaylist={setSelectedPlaylist}
            />
          </>
        )}

        <MusicPlayer selectedTrack={selectedTrack} />
      </>
    );
  }

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
      <button
        type="button"
        className="playlist-open-button"
        onClick={() => setShowPlaylists(true)}
      >
        My Playlists
      </button>

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
