-- SCRUM-191: Soundwaves Playlist E2E Test Fixture
-- Development/test database only.
-- Safe to rerun without creating duplicate QA records.

USE soundwave;

START TRANSACTION;

-- Create a dedicated QA user if one does not exist.
-- Password hash is a placeholder: do not use this account
-- for authentication or login testing.
INSERT INTO users (username, email, password_hash, role)
SELECT
    'soundwaves_e2e_qa',
    'soundwaves-e2e-qa@example.invalid',
    'NOT_A_VALID_PASSWORD_HASH',
    'user'
WHERE NOT EXISTS (
    SELECT 1
    FROM users
    WHERE username = 'soundwaves_e2e_qa'
);

-- Create the QA playlist under the test user.
INSERT INTO playlists (user_id, name, description)
SELECT
    u.id,
    'Sprint 5 QA Playlist',
    'Dedicated playlist fixture for SCRUM-191'
FROM users u
WHERE u.username = 'soundwaves_e2e_qa'
  AND NOT EXISTS (
      SELECT 1
      FROM playlists p
      WHERE p.user_id = u.id
        AND p.name = 'Sprint 5 QA Playlist'
  );

-- Add the existing Demo Tone A track (ID 3).
INSERT INTO playlist_tracks (playlist_id, track_id, position)
SELECT
    p.id,
    t.id,
    1
FROM playlists p
JOIN users u ON p.user_id = u.id
JOIN tracks t ON t.id = 3
WHERE u.username = 'soundwaves_e2e_qa'
  AND p.name = 'Sprint 5 QA Playlist'
  AND NOT EXISTS (
      SELECT 1
      FROM playlist_tracks pt
      WHERE pt.playlist_id = p.id
        AND pt.track_id = t.id
  );

COMMIT;

-- Read-only verification
SELECT u.username, p.id AS playlist_id, p.name,
       t.id AS track_id, t.title
FROM users u
JOIN playlists p ON p.user_id = u.id
LEFT JOIN playlist_tracks pt ON pt.playlist_id = p.id
LEFT JOIN tracks t ON t.id = pt.track_id
WHERE u.username = 'soundwaves_e2e_qa';