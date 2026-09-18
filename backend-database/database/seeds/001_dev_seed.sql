USE soundwave;

-- Development-only sample catalog data.
-- Password values are intentionally not seeded here.

INSERT INTO artists (name)
VALUES ('Demo Artist');

INSERT INTO albums (artist_id, title, release_date, artwork_path)
SELECT id, 'Demo Album', '2026-01-01', '/media/artwork/demo-album.jpg'
FROM artists
WHERE name = 'Demo Artist'
LIMIT 1;

INSERT INTO tracks (album_id, artist_id, title, duration_ms, track_number, file_path, mime_type)
SELECT a.id, ar.id, 'Demo Track', 180000, 1, '/media/audio/demo-track.mp3', 'audio/mpeg'
FROM albums a
JOIN artists ar ON ar.id = a.artist_id
WHERE a.title = 'Demo Album' AND ar.name = 'Demo Artist'
LIMIT 1;
