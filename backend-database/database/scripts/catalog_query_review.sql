-- SCRUM-97: Soundwaves Catalog Query Review (MySQL 8.x)
-- READ-ONLY: SELECT, EXPLAIN, SHOW statements only.
-- Run after 001_schema.sql and 002_dev_seed.sql have been loaded.
-- EXPLAIN shows estimated plans; no timings are claimed by this script.
USE soundwave;

-- 00. Seed size and index inventory
SELECT 'artists' AS entity, COUNT(*) AS row_count FROM artists
UNION ALL SELECT 'albums', COUNT(*) FROM albums
UNION ALL SELECT 'tracks', COUNT(*) FROM tracks;
SHOW INDEX FROM artists;
SHOW INDEX FROM albums;
SHOW INDEX FROM tracks;

-- 01. Artist -> Albums: existing index idx_albums_artist_id
EXPLAIN SELECT id, artist_id, title, release_date, artwork_path
FROM albums WHERE artist_id = 6 ORDER BY id;
SELECT id, artist_id, title, release_date, artwork_path
FROM albums WHERE artist_id = 6 ORDER BY id;

-- 02. Album -> Tracks: existing index idx_tracks_album_id
-- ORDER BY track_number may require sorting; measure before adding a composite index.
EXPLAIN SELECT id, album_id, artist_id, title, track_number, duration_ms
FROM tracks WHERE album_id = 6 ORDER BY track_number, id;
SELECT id, album_id, artist_id, title, track_number, duration_ms
FROM tracks WHERE album_id = 6 ORDER BY track_number, id;

-- 03. Artist -> Tracks: existing index idx_tracks_artist_id
EXPLAIN SELECT id, album_id, artist_id, title
FROM tracks WHERE artist_id = 6 ORDER BY id;
SELECT id, album_id, artist_id, title
FROM tracks WHERE artist_id = 6 ORDER BY id;

-- 04. Track detail with album + track artist in a single query.
-- LEFT JOIN is intentional: album_id and artist_id are nullable.
-- Tracks.artist_id is the track's artist; it need not equal albums.artist_id.
EXPLAIN SELECT t.id, t.title, t.track_number, t.duration_ms,
       al.id AS album_id, al.title AS album_title,
       ar.id AS artist_id, ar.name AS artist_name
FROM tracks AS t
LEFT JOIN albums AS al ON al.id = t.album_id
LEFT JOIN artists AS ar ON ar.id = t.artist_id
WHERE t.id = 16;
SELECT t.id, t.title, t.track_number, t.duration_ms,
       al.id AS album_id, al.title AS album_title,
       ar.id AS artist_id, ar.name AS artist_name
FROM tracks AS t
LEFT JOIN albums AS al ON al.id = t.album_id
LEFT JOIN artists AS ar ON ar.id = t.artist_id
WHERE t.id = 16;

-- 05. Search: approximate EF Core Contains(query) semantics using LIKE.
-- The leading % typically prevents a conventional B-tree prefix lookup.
-- SQL collation determines case/accent sensitivity; verify actual EF SQL separately.
EXPLAIN SELECT id, title FROM tracks WHERE title LIKE '%Midnight%';
SELECT id, title FROM tracks WHERE title LIKE '%Midnight%' ORDER BY id;
EXPLAIN SELECT id, title FROM albums WHERE title LIKE '%Midnight%';
SELECT id, title FROM albums WHERE title LIKE '%Midnight%' ORDER BY id;
EXPLAIN SELECT id, name FROM artists WHERE name LIKE '%Midnight%';
SELECT id, name FROM artists WHERE name LIKE '%Midnight%' ORDER BY id;

-- 06. Relationship consistency diagnostics (expected zero for this seed).
-- Foreign keys already protect against missing referenced IDs.
SELECT 'orphan_albums' AS check_name, COUNT(*) AS issue_count
FROM albums al LEFT JOIN artists ar ON ar.id = al.artist_id
WHERE al.artist_id IS NOT NULL AND ar.id IS NULL
UNION ALL
SELECT 'orphan_track_albums', COUNT(*)
FROM tracks t LEFT JOIN albums al ON al.id = t.album_id
WHERE t.album_id IS NOT NULL AND al.id IS NULL
UNION ALL
SELECT 'orphan_track_artists', COUNT(*)
FROM tracks t LEFT JOIN artists ar ON ar.id = t.artist_id
WHERE t.artist_id IS NOT NULL AND ar.id IS NULL;

-- Optional seed-specific observation (NOT a general integrity requirement):
-- Tracks can legally have a different artist from their album artist.
SELECT COUNT(*) AS differing_track_and_album_artist_count
FROM tracks t JOIN albums al ON al.id = t.album_id
WHERE t.artist_id IS NOT NULL AND al.artist_id IS NOT NULL
  AND t.artist_id <> al.artist_id;

-- 07. Catalog browse query shape: observe full listing vs bounded listing.
-- Backend GetArtistsAsync/GetAlbumsAsync currently materialize all rows.
EXPLAIN SELECT id, name FROM artists ORDER BY id LIMIT 20;
EXPLAIN SELECT id, artist_id, title FROM albums ORDER BY id LIMIT 20;
EXPLAIN SELECT id, album_id, artist_id, title FROM tracks ORDER BY id LIMIT 20;

-- NOTE: Repeated API round trips and N+1 behavior cannot be established from SQL
-- alone. Review backend request traces/logs with the backend teammate.
