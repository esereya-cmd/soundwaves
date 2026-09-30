-- SCRUM-69: Soundwaves database schema validation
-- Checks whether the required database tables exist.

USE soundwave;

SELECT table_name
FROM information_schema.tables
WHERE table_schema = DATABASE()
  AND table_name IN (
    'users',
    'artists',
    'albums',
    'tracks',
    'playlists',
    'playlist_tracks',
    'favorites',
    'playback_history',
    'auth_sessions'
  )
ORDER BY table_name;

SELECT COUNT(*) AS required_tables_found
FROM information_schema.tables
WHERE table_schema = DATABASE()
  AND table_name IN (
    'users',
    'artists',
    'albums',
    'tracks',
    'playlists',
    'playlist_tracks',
    'favorites',
    'playback_history',
    'auth_sessions'
  );
