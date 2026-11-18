
import { useState } from "react";

function PlaylistControls({ playlist, onRename, onDelete }) {
  const [newName, setNewName] = useState(playlist.name);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleRename(event) {
    event.preventDefault();

    const name = newName.trim();

    if (name === "") {
      setError("Please enter a playlist name.");
      return;
    }

    if (isSubmitting) {
      return;
    }

    setError("");
    setIsSubmitting(true);

    try {
      if (!onRename) {
        throw new Error("Rename API is unavailable.");
      }

      await onRename(playlist.id, name);
    } catch {
      setError("Unable to rename playlist. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  }

  async function handleDelete() {
    if (isSubmitting) {
      return;
    }

    const confirmed = window.confirm(
      `Are you sure you want to delete "${playlist.name}"?`
    );

    if (!confirmed) {
      return;
    }

    setError("");
    setIsSubmitting(true);

    try {
      if (!onDelete) {
        throw new Error("Delete API is unavailable.");
      }

      await onDelete(playlist.id);
    } catch {
      setError("Unable to delete playlist. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="playlist-controls">
      <h2>Manage Playlist</h2>

      <form onSubmit={handleRename}>
        <label htmlFor="rename-playlist">
          Playlist Name:
        </label>

        <input
          id="rename-playlist"
          type="text"
          value={newName}
          onChange={(event) => setNewName(event.target.value)}
          disabled={isSubmitting}
        />

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Saving..." : "Rename Playlist"}
        </button>
      </form>

      <button
        type="button"
        onClick={handleDelete}
        disabled={isSubmitting}
      >
        Delete Playlist
      </button>

      {error && <p role="alert">{error}</p>}
    </div>
  );
}

export default PlaylistControls;
