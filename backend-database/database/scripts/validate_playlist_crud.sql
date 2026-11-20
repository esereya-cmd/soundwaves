-- SCRUM-187: Validate Playlist CRUD Persistence
-- Soundwaves / MySQL
-- Run against development database only.

START TRANSACTION;

-- Baseline
SELECT COUNT(*) AS before_playlists
FROM playlists;

SELECT COUNT(*) AS before_memberships
FROM playlist_tracks;

-- CREATE
INSERT INTO playlists (user_id, name, description)
VALUES (
    2,
    '[TEST-S187] CRUD Validation',
    'Temporary CRUD test playlist'
);

SET @test_id = LAST_INSERT_ID();

-- READ
SELECT id, name, description
FROM playlists
WHERE id = @test_id;

-- ADD TRACK
INSERT INTO playlist_tracks (
    playlist_id,
    track_id,
    position
)
VALUES (@test_id, 1, 1);

-- UPDATE
UPDATE playlists
SET name = '[TEST-S187] Updated Playlist',
    description = 'Updated successfully'
WHERE id = @test_id;

-- VERIFY UPDATE
SELECT id, name, description
FROM playlists
WHERE id = @test_id;

-- DELETE
DELETE FROM playlists
WHERE id = @test_id;

-- VERIFY DELETION
SELECT COUNT(*) AS remaining_playlists
FROM playlists
WHERE id = @test_id;

SELECT COUNT(*) AS remaining_memberships
FROM playlist_tracks
WHERE playlist_id = @test_id;

-- VERIFY UNRELATED DATA
SELECT id, name
FROM playlists
WHERE id IN (2, 3, 4)
ORDER BY id;

-- RESTORE
ROLLBACK;

-- FINAL VALIDATION
SELECT COUNT(*) AS final_playlists
FROM playlists;

SELECT COUNT(*) AS final_memberships
FROM playlist_tracks;