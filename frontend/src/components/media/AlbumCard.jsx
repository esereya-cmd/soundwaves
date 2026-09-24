function AlbumCard({ title, artist }) {
  return (
    <article className="catalog-card">
      <h3>{title}</h3>
      <p>{artist}</p>
    </article>
  );
}

export default AlbumCard;