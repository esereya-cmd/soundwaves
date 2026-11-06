USE soundwave;

-- SCRUM-125: Safe audio fixture mapping
-- Supports both the small Demo Track database
-- and the expanded 120-track development database.
-- Does not delete or replace existing records.

START TRANSACTION;

-- Create Nova Lane only if missing.
INSERT INTO artists (name)
SELECT 'Nova Lane'
WHERE NOT EXISTS (
    SELECT 1 FROM artists
    WHERE name = 'Nova Lane'
);

-- Create Late Nights only if missing.
INSERT INTO albums (
    artist_id,
    title,
    release_date,
    artwork_path
)
SELECT
    ar.id,
    'Late Nights',
    '2026-01-01',
    '/artwork/late-nights.jpg'
FROM artists ar
WHERE ar.name = 'Nova Lane'
  AND NOT EXISTS (
      SELECT 1
      FROM albums a
      WHERE a.artist_id = ar.id
        AND a.title = 'Late Nights'
  )
ORDER BY ar.id
LIMIT 1;

-- Create Midnight Drive only if missing.
INSERT INTO tracks (
    album_id,
    artist_id,
    title,
    track_number,
    duration_ms,
    file_path,
    mime_type
)
SELECT
    a.id,
    ar.id,
    'Midnight Drive',
    1,
    222000,
    'demo-tone-a.mp3',
    'audio/mpeg'
FROM albums a
JOIN artists ar ON ar.id = a.artist_id
WHERE ar.name = 'Nova Lane'
  AND a.title = 'Late Nights'
  AND NOT EXISTS (
      SELECT 1
      FROM tracks t
      WHERE t.album_id = a.id
        AND t.title = 'Midnight Drive'
  )
ORDER BY a.id
LIMIT 1;

-- Create Neon Rain only if missing.
INSERT INTO tracks (
    album_id,
    artist_id,
    title,
    track_number,
    duration_ms,
    file_path,
    mime_type
)
SELECT
    a.id,
    ar.id,
    'Neon Rain',
    2,
    198000,
    'demo-tone-b.mp3',
    'audio/mpeg'
FROM albums a
JOIN artists ar ON ar.id = a.artist_id
WHERE ar.name = 'Nova Lane'
  AND a.title = 'Late Nights'
  AND NOT EXISTS (
      SELECT 1
      FROM tracks t
      WHERE t.album_id = a.id
        AND t.title = 'Neon Rain'
  )
ORDER BY a.id
LIMIT 1;

-- Update existing records using their identities,
-- not hardcoded track IDs.
UPDATE tracks t
JOIN albums a ON a.id = t.album_id
JOIN artists ar ON ar.id = a.artist_id
SET
    t.file_path = CASE
        WHEN t.title = 'Midnight Drive'
            THEN 'demo-tone-a.mp3'
        WHEN t.title = 'Neon Rain'
            THEN 'demo-tone-b.mp3'
    END,
    t.mime_type = 'audio/mpeg'
WHERE ar.name = 'Nova Lane'
  AND a.title = 'Late Nights'
  AND t.title IN ('Midnight Drive', 'Neon Rain');

COMMIT;

-- Verify the final mappings.
SELECT
    t.id,
    t.title,
    ar.name AS artist,
    a.title AS album,
    t.file_path,
    t.mime_type
FROM tracks t
JOIN albums a ON a.id = t.album_id
JOIN artists ar ON ar.id = a.artist_id
WHERE ar.name = 'Nova Lane'
  AND a.title = 'Late Nights'
  AND t.title IN ('Midnight Drive', 'Neon Rain')
ORDER BY t.id;
