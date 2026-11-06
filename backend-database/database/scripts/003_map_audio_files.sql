USE soundwave;

-- SCRUM-125: Map Track Metadata to Real Audio Files
-- Update only the two selected demo tracks.

UPDATE tracks
SET
    file_path = 'demo-track-01.mp3',
    mime_type = 'audio/mpeg'
WHERE id = 1;

UPDATE tracks
SET
    file_path = 'demo-track-02.mp3',
    mime_type = 'audio/mpeg'
WHERE id = 2;

-- Verify the mappings.
SELECT
    id,
    title,
    album_id,
    artist_id,
    file_path,
    mime_type
FROM tracks
WHERE id IN (1, 2);
