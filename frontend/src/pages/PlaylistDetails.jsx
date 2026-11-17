
function PlaylistDetails({
  playlist,
  onBack,
  onSelectTrack,
  isLoading = false,
  error = "",
}) {
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
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}

export default PlaylistDetails;
