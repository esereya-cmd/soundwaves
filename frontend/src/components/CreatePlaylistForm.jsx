
import { useState } from "react";

function CreatePlaylistForm({ onCreatePlaylist }) {
  const [name, setName] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event) {
    event.preventDefault();

    const playlistName = name.trim();

    if (playlistName === "") {
      setError("Please enter a playlist name.");
      return;
    }

    if (isSubmitting) {
      return;
    }

    setError("");
    setIsSubmitting(true);

    try {
      if (!onCreatePlaylist) {
        throw new Error("Playlist creation is not available yet.");
      }

      await onCreatePlaylist(playlistName);
      setName("");
    } catch {
      setError("Unable to create playlist. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h2>Create Playlist</h2>

      <label htmlFor="playlist-name">Playlist Name:</label>

      <input
        id="playlist-name"
        type="text"
        value={name}
        onChange={(event) => setName(event.target.value)}
        disabled={isSubmitting}
      />

      {error && <p role="alert">{error}</p>}

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Creating..." : "Create Playlist"}
      </button>
    </form>
  );
}

export default CreatePlaylistForm;
