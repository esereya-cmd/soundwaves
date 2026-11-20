
-- SCRUM-186
-- Soundwaves Playlist Integrity Validation
-- Read-only, repeatable checks.
-- Expected: zero duplicates and zero orphan references.

-- Test 1: Duplicate playlist-track pairs
SELECT
    playlist_id,
    track_id,
    COUNT(*) AS membership_count
FROM playlist_tracks
GROUP BY playlist_id, track_id
HAVING COUNT(*) > 1;

-- Test 2: Missing playlist references
SELECT pt.*
FROM playlist_tracks pt
LEFT JOIN playlists p
    ON pt.playlist_id = p.id
WHERE p.id IS NULL;

-- Test 3: Missing track references
SELECT pt.*
FROM playlist_tracks pt
LEFT JOIN tracks t
    ON pt.track_id = t.id
WHERE t.id IS NULL;

-- Test 4: Foreign key definitions
SELECT
    CONSTRAINT_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM information_schema.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = 'playlist_tracks'
  AND REFERENCED_TABLE_NAME IS NOT NULL;

-- Test 5: Final integrity summary
SELECT
    (SELECT COUNT(*)
     FROM playlist_tracks) AS total_memberships,

    (SELECT COUNT(*)
     FROM playlist_tracks pt
     LEFT JOIN playlists p
       ON pt.playlist_id = p.id
     WHERE p.id IS NULL) AS orphan_playlists,

    (SELECT COUNT(*)
     FROM playlist_tracks pt
     LEFT JOIN tracks t
       ON pt.track_id = t.id
     WHERE t.id IS NULL) AS orphan_tracks;
