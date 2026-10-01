USE soundwave;

SELECT COUNT(*) AS demo_artist_found
FROM artists
WHERE name = 'Demo Artist';

SELECT COUNT(*) AS demo_album_found
FROM albums a
JOIN artists ar ON ar.id = a.artist_id
WHERE a.title = 'Demo Album'
  AND ar.name = 'Demo Artist';

SELECT COUNT(*) AS demo_track_found
FROM tracks t
JOIN albums a ON a.id = t.album_id
JOIN artists ar ON ar.id = t.artist_id
WHERE t.title = 'Demo Track'
  AND a.title = 'Demo Album'
  AND ar.name = 'Demo Artist';
  