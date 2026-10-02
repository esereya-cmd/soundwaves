function SongCard({ title, artistId, albumId, onSelect }) {
  return (
    <article className="catalog-card">
      <h3>{title}</h3>

      {artistId && <p>Artist ID: {artistId}</p>}
      {albumId && <p>Album ID: {albumId}</p>}

      <button type="button" onClick={onSelect}>
        View Details
      </button>
    </article>
  );
}

export default SongCard;
