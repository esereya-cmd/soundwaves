import { useState } from "react";

function AddToPlaylist({ track, playlists = [], onAddTrack, onCancel }) {
  const [playlistId, setPlaylistId] = useState("");
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event) {
    event.preventDefault();

    if (playlistId === "") {
      setError("Please select a playlist.");
      return;
    }

    if (isSubmitting) {
      return;
    }

    setError("");
    setIsSubmitting(true);

    try {
      if (!onAddTrack) {
        throw new Error("Add-to-playlist API unavailable.");
      }

      await onAddTrack(Number(playlistId), track.id);
    } catch {
      setError("Unable to add track to playlist. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <section className="add-to-playlist">
      <h2>Add to Playlist</h2>

      <p>Selected Track: {track.title}</p>

      <form onSubmit={handleSubmit}>
        <label htmlFor="choose-playlist">Choose Playlist:</label>

        <select
          id="choose-playlist"
          value={playlistId}
          onChange={(event) => setPlaylistId(event.target.value)}
          disabled={isSubmitting}
        >
          <option value="">Select a playlist</option>

          {playlists.map((playlist) => (
            <option key={playlist.id} value={playlist.id}>
              {playlist.name}
            </option>
          ))}
        </select>

        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Adding..." : "Add Track"}
        </button>

        <button type="button" onClick={onCancel} disabled={isSubmitting}>
          Cancel
        </button>
      </form>

      {error && <p role="alert">{error}</p>}
    </section>
  );
}

export default AddToPlaylist;
