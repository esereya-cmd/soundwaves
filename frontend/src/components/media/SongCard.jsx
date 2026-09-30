function SongCard({ title, artist, album, onSelect }) {
  return (
    <article className="catalog-card">
      <h3>{title}</h3>
      <p>{artist}</p>
      <p>{album}</p>

      <button type="button" onClick={onSelect}>
        View Details
      </button>
    </article>
  );
}

export default SongCard;

