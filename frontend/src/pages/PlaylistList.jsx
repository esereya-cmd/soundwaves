
function PlaylistList({ playlists = [], onSelectPlaylist }) {
  return (
    <main className="playlist-page">
      <h1>My Playlists</h1>

      {playlists.length === 0 ? (
        <p>No playlists yet.</p>
      ) : (
        <section className="playlist-grid">
          {playlists.map((playlist) => (
            <div className="playlist-card" key={playlist.id}>
              <h2>{playlist.name}</h2>

              <button
                type="button"
                onClick={() => onSelectPlaylist?.(playlist)}
              >
                View Playlist
              </button>
            </div>
          ))}
        </section>
      )}
    </main>
  );
}

export default PlaylistList;

