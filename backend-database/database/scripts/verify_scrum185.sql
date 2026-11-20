SELECT u.id AS fixture_user_id, u.username, u.is_active, p.id AS playlist_id, p.name, COUNT(pt.track_id) AS track_count
FROM users u JOIN playlists p ON p.user_id=u.id
LEFT JOIN playlist_tracks pt ON pt.playlist_id=p.id
WHERE u.username='scrum185_fixture' AND p.name LIKE '[TEST-S185]%'
GROUP BY u.id,u.username,u.is_active,p.id,p.name ORDER BY p.name;
SELECT p.id AS playlist_id,p.name,pt.position,pt.track_id,t.title
FROM playlists p JOIN users u ON u.id=p.user_id
JOIN playlist_tracks pt ON pt.playlist_id=p.id JOIN tracks t ON t.id=pt.track_id
WHERE u.username='scrum185_fixture' AND p.name LIKE '[TEST-S185]%'
ORDER BY p.name,pt.position;
SELECT COUNT(*) AS orphan_memberships FROM playlist_tracks pt
LEFT JOIN playlists p ON p.id=pt.playlist_id LEFT JOIN tracks t ON t.id=pt.track_id
WHERE p.id IS NULL OR t.id IS NULL;
