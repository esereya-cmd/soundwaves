
import { useState } from "react";

import Catalog from "./pages/Catalog";
import SongDetails from "./pages/SongDetails";
import AlbumDetails from "./pages/AlbumDetails";
import ArtistDetails from "./pages/ArtistDetails";
import PlaylistList from "./pages/PlaylistList";
import PlaylistDetails from "./pages/PlaylistDetails";
import CreatePlaylistForm from "./components/CreatePlaylistForm";
import MusicPlayer from "./components/media/MusicPlayer";

import "./styles/playlist.css";

// Temporary playlist data until the frontend is connected
// to the backend Playlist API.
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

  // SCRUM-168: Create Playlist
  // Backend integration is still pending.
  async function handleCreatePlaylist() {
    throw new Error("Playlist creation API is not connected yet.");
  }

  // SCRUM-170: Add Track to Playlist
  // Backend integration is still pending.
  async function handleAddTrack() {
    throw new Error("Add-to-playlist API is not connected yet.");
  }

  // SCRUM-171: Remove Track from Playlist
  // The UI is ready, but backend integration must be verified.
  async function handleRemoveTrack() {
    throw new Error("Remove-track backend integration is pending.");
  }

  // Playlist pages
  if (showPlaylists) {
    return (
      <>
        {selectedPlaylist ? (
          <PlaylistDetails
            playlist={selectedPlaylist}
            onBack={() => setSelectedPlaylist(null)}
            onSelectTrack={setSelectedTrack}
            onRemoveTrack={handleRemoveTrack}
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

            <CreatePlaylistForm
              onCreatePlaylist={handleCreatePlaylist}
            />

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

  // Artist details
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

  // Album details
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

  // Song details
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

  // Main music catalog
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
        playlists={samplePlaylists}
        onAddTrack={handleAddTrack}
      />

      <MusicPlayer selectedTrack={selectedTrack} />
    </>
  );
}

export default App;
