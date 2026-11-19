
import { useState } from "react";
import PlaylistControls from "../components/PlaylistControls";

function PlaylistDetails({
  playlist,
  onBack,
  onSelectTrack,
  isLoading = false,
  error = "",
  onRenamePlaylist,
  onDeletePlaylist,
  onRemoveTrack,
}) {
  const [trackToRemove, setTrackToRemove] = useState(null);
  const [isRemoving, setIsRemoving] = useState(false);
  const [removeMessage, setRemoveMessage] = useState("");
  const [removeError, setRemoveError] = useState("");

  async function handleRemoveTrack() {
    if (!trackToRemove || isRemoving) {
      return;
    }

    setIsRemoving(true);
    setRemoveError("");
    setRemoveMessage("");

    try {
      if (!onRemoveTrack) {
        throw new Error("Remove-track API is not connected.");
      }

      await onRemoveTrack(playlist.id, trackToRemove.id);

      setRemoveMessage(
        "Remove request completed. Refresh playlist details to confirm."
      );
      setTrackToRemove(null);
    } catch {
      setRemoveError(
        "Unable to remove track from playlist. Please try again."
      );
    } finally {
      setIsRemoving(false);
    }
  }

  if (isLoading) {
    return (
      <main className="playlist-details">
        <button type="button" onClick={onBack}>
          Back to Playlists
        </button>

        <h1>Playlist Details</h1>
        <p role="status">Loading playlist...</p>
      </main>
    );
  }

  if (error) {
    return (
      <main className="playlist-details">
        <button type="button" onClick={onBack}>
          Back to Playlists
        </button>

        <h1>Playlist Details</h1>
        <p role="alert">{error}</p>
      </main>
    );
  }

  if (!playlist) {
    return (
      <main className="playlist-details">
        <button type="button" onClick={onBack}>
          Back to Playlists
        </button>

        <h1>Playlist Details</h1>
        <p>No playlist selected.</p>
      </main>
    );
  }

  const tracks = Array.isArray(playlist.tracks)
    ? playlist.tracks
    : [];

  return (
    <main className="playlist-details">
      <button type="button" onClick={onBack}>
        Back to Playlists
      </button>

      <h1>{playlist.name}</h1>
      <h2>Tracks</h2>

      {tracks.length === 0 ? (
        <p>No tracks in this playlist.</p>
      ) : (
        <ul className="playlist-tracks">
          {tracks.map((track) => (
            <li key={track.id}>
              <button
                type="button"
                onClick={() => onSelectTrack?.(track)}
              >
                {track.title}
              </button>

              <button
                type="button"
                disabled={isRemoving}
                onClick={() => {
                  setTrackToRemove(track);
                  setRemoveError("");
                  setRemoveMessage("");
                }}
              >
                Remove Track
              </button>
            </li>
          ))}
        </ul>
      )}

      {trackToRemove && (
        <section className="playlist-remove-confirmation">
          <p>
            Remove {trackToRemove.title} from this playlist?
          </p>

          <button
            type="button"
            disabled={isRemoving}
            onClick={handleRemoveTrack}
          >
            {isRemoving ? "Removing..." : "Confirm Remove"}
          </button>

          <button
            type="button"
            disabled={isRemoving}
            onClick={() => {
              setTrackToRemove(null);
              setRemoveError("");
            }}
          >
            Cancel
          </button>
        </section>
      )}

      {removeError && <p role="alert">{removeError}</p>}
      {removeMessage && <p role="status">{removeMessage}</p>}

      <PlaylistControls
        playlist={playlist}
        onRename={onRenamePlaylist}
        onDelete={onDeletePlaylist}
      />
    </main>
  );
}

export default PlaylistDetails;
