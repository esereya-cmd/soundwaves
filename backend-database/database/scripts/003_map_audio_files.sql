USE soundwave;

-- SCRUM-125: Map Track Metadata to Real Audio Files
-- Track 1: Midnight Drive
-- Track 2: Neon Rain

UPDATE tracks
SET
    file_path = 'demo-track-01.mp3',
    mime_type = 'audio/mpeg'
WHERE id = 1
  AND title = 'Midnight Drive';

UPDATE tracks
SET
    file_path = 'demo-track-02.mp3',
    mime_type = 'audio/mpeg'
WHERE id = 2
  AND title = 'Neon Rain';

-- Verify updated metadata
SELECT
    id,
    title,
    album_id,
    artist_id,
    file_path,
    mime_type
FROM tracks
WHERE id IN (1, 2);
