function SongCard({ title, artist, album }) {
  return (
    <article className="catalog-card">
      <h3>{title}</h3>
      <p>{artist}</p>
      <p>{album}</p>
    </article>
  );
}

export default SongCard;