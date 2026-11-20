
import { useEffect, useState } from "react";

import Catalog from "./pages/Catalog";
import SongDetails from "./pages/SongDetails";
import AlbumDetails from "./pages/AlbumDetails";
import ArtistDetails from "./pages/ArtistDetails";
import PlaylistList from "./pages/PlaylistList";
import PlaylistDetails from "./pages/PlaylistDetails";
import CreatePlaylistForm from "./components/CreatePlaylistForm";
import MusicPlayer from "./components/media/MusicPlayer";
import {
  getPlaylists,
  getPlaylist,
  createPlaylist,
  updatePlaylist,
  deletePlaylist,
  addTrack,
  removeTrack,
} from "./services/playlistApi";

import "./styles/playlist.css";

function App() {
  const [selectedSong, setSelectedSong] = useState(null);
  const [selectedAlbum, setSelectedAlbum] = useState(null);
  const [selectedArtist, setSelectedArtist] = useState(null);
  const [selectedTrack, setSelectedTrack] = useState(null);

  const [showPlaylists, setShowPlaylists] = useState(false);
  const [selectedPlaylist, setSelectedPlaylist] = useState(null);

  const [playlists, setPlaylists] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [playlistError, setPlaylistError] = useState("");
  const [detailsLoading, setDetailsLoading] = useState(false);
  const [detailsError, setDetailsError] = useState("");

  // Load real playlists from the backend.
  async function loadPlaylists() {
    setIsLoading(true);
    setPlaylistError("");

    try {
      const data = await playlistApi.getAll();
      setPlaylists(Array.isArray(data) ? data : []);
    } catch {
      setPlaylistError("Unable to load playlists.");
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    loadPlaylists();
  }, []);

  // Retrieve playlist details, including tracks.
  async function openPlaylist(id) {
    setDetailsLoading(true);
    setDetailsError("");

    try {
      const playlist = await playlistApi.getById(id);
      setSelectedPlaylist(playlist);
    } catch {
      setDetailsError("Unable to load playlist details.");
    } finally {
      setDetailsLoading(false);
    }
  }

  // Create a playlist.
  async function handleCreatePlaylist(name) {
    await playlistApi.create(name);
    await loadPlaylists();
  }

  // Rename an existing playlist.
  async function handleRenamePlaylist(id, name) {
    const existing = await playlistApi.getById(id);

    await playlistApi.update(
      id,
      name,
      existing.description
    );

    await loadPlaylists();
    await openPlaylist(id);
  }

  // Delete a playlist.
  async function handleDeletePlaylist(id) {
    await playlistApi.delete(id);
    setSelectedPlaylist(null);
    await loadPlaylists();
  }

  // Add an existing catalog track.
  async function handleAddTrack(playlistId, trackId) {
    await playlistApi.addTrack(playlistId, trackId);
    await loadPlaylists();

    if (selectedPlaylist?.id === playlistId) {
      await openPlaylist(playlistId);
    }
  }

  // Remove a track without deleting catalog music.
  async function handleRemoveTrack(playlistId, trackId) {
    await playlistApi.removeTrack(playlistId, trackId);
    await openPlaylist(playlistId);
  }

  // Playlist pages.
  if (showPlaylists) {
    return (
      <>
        {selectedPlaylist || detailsLoading || detailsError ? (
          <PlaylistDetails
            key={selectedPlaylist?.id ?? "loading"}
            playlist={selectedPlaylist}
            isLoading={detailsLoading}
            error={detailsError}
            onBack={() => {
              setSelectedPlaylist(null);
              setDetailsError("");
            }}
            onSelectTrack={setSelectedTrack}
            onRenamePlaylist={handleRenamePlaylist}
            onDeletePlaylist={handleDeletePlaylist}
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
              playlists={playlists}
              isLoading={isLoading}
              error={playlistError}
              onSelectPlaylist={(playlist) =>
                openPlaylist(playlist.id)
              }
            />
          </>
        )}

        <MusicPlayer selectedTrack={selectedTrack} />
      </>
    );
  }

  // Artist details.
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

  // Album details.
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

  // Song details.
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

  // Main catalog.
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
        playlists={playlists}
        onAddTrack={handleAddTrack}
      />

      <MusicPlayer selectedTrack={selectedTrack} />
    </>
  );
}

export default App;
