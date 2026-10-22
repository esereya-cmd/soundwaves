
function ArtistCard({ name, onSelect }) {
  return (
    <article className="catalog-card">
      <h3>{name}</h3>

      {onSelect && (
        <button type="button" onClick={onSelect}>
          View Artist
        </button>
      )}
    </article>
  );
}

export default ArtistCard;
