function AlbumCard({ title, artist, onSelect }) {
  return (
    <article className="catalog-card">
      <h3>{title}</h3>
      <p>{artist}</p>

      <button type="button" onClick={onSelect}>
        View Album
      </button>
    </article>
  );
}

export default AlbumCard;
