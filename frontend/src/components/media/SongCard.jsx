function SongCard({
  title,
  artistId,
  albumId,
  onSelect,
  onAddToPlaylist,
}) {
  return (
    <article className="catalog-card">
      <h3>{title}</h3>

      {artistId && <p>Artist ID: {artistId}</p>}
      {albumId && <p>Album ID: {albumId}</p>}

      <button type="button" onClick={onSelect}>
        View Details
      </button>

      {onAddToPlaylist && (
        <button type="button" onClick={onAddToPlaylist}>
          Add to Playlist
        </button>
      )}
    </article>
  );
}

export default SongCard;
