-- SCRUM-185: local development only. Requires existing catalog track IDs 1..4.
-- Run in MySQL 8.4. This script creates no schema objects.
-- Fixture account uses a deliberately unusable password hash; do not use for login testing.
DELIMITER $$
DROP PROCEDURE IF EXISTS scrum185_install_fixtures$$
CREATE PROCEDURE scrum185_install_fixtures()
BEGIN
  DECLARE v_user BIGINT UNSIGNED;
  DECLARE v_empty BIGINT UNSIGNED;
  DECLARE v_basic BIGINT UNSIGNED;
  DECLARE v_ordered BIGINT UNSIGNED;
  DECLARE v_count INT;
  DECLARE v_conflicts INT;
  DECLARE v_name VARCHAR(255);
  DECLARE v_email VARCHAR(255);
  DECLARE EXIT HANDLER FOR SQLEXCEPTION
  BEGIN
    ROLLBACK;
    RESIGNAL;
  END;
  START TRANSACTION;
  IF (SELECT COUNT(*) FROM tracks WHERE id IN (1,2,3,4)) <> 4 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='SCRUM-185 requires catalog track IDs 1, 2, 3, 4';
  END IF;
  SET v_name='scrum185_fixture';
  SET v_email='scrum185-fixture@example.invalid';
  SELECT COUNT(*) INTO v_conflicts FROM users
    WHERE (username=v_name OR email=v_email)
      AND NOT (username=v_name AND email=v_email AND password_hash='!SCRUM185_DISABLED!' AND role='user' AND is_active=0);
  IF v_conflicts <> 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='SCRUM-185 fixture user conflicts with an existing account';
  END IF;
  INSERT INTO users (username,email,password_hash,role,is_active)
  SELECT v_name,v_email,'!SCRUM185_DISABLED!','user',0
  WHERE NOT EXISTS (SELECT 1 FROM users WHERE username=v_name AND email=v_email);
  SELECT id INTO v_user FROM users WHERE username=v_name AND email=v_email;
  SELECT COUNT(*) INTO v_conflicts FROM playlists
    WHERE name IN ('[TEST-S185] Empty Playlist','[TEST-S185] Basic Playlist','[TEST-S185] Ordered Playlist')
      AND user_id <> v_user;
  IF v_conflicts <> 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='SCRUM-185 fixture playlist name belongs to another user';
  END IF;
  SELECT COUNT(*) INTO v_conflicts FROM (
    SELECT name FROM playlists WHERE user_id=v_user AND name IN
      ('[TEST-S185] Empty Playlist','[TEST-S185] Basic Playlist','[TEST-S185] Ordered Playlist')
    GROUP BY name HAVING COUNT(*) > 1
  ) AS duplicates;
  IF v_conflicts <> 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='SCRUM-185 duplicate fixture playlist names detected';
  END IF;
  INSERT INTO playlists (user_id,name,description)
  SELECT v_user,'[TEST-S185] Empty Playlist','SCRUM-185 deterministic fixture; no tracks'
  WHERE NOT EXISTS (SELECT 1 FROM playlists WHERE user_id=v_user AND name='[TEST-S185] Empty Playlist');
  INSERT INTO playlists (user_id,name,description)
  SELECT v_user,'[TEST-S185] Basic Playlist','SCRUM-185 deterministic fixture; three tracks'
  WHERE NOT EXISTS (SELECT 1 FROM playlists WHERE user_id=v_user AND name='[TEST-S185] Basic Playlist');
  INSERT INTO playlists (user_id,name,description)
  SELECT v_user,'[TEST-S185] Ordered Playlist','SCRUM-185 deterministic fixture; four tracks'
  WHERE NOT EXISTS (SELECT 1 FROM playlists WHERE user_id=v_user AND name='[TEST-S185] Ordered Playlist');
  SELECT id INTO v_empty FROM playlists WHERE user_id=v_user AND name='[TEST-S185] Empty Playlist';
  SELECT id INTO v_basic FROM playlists WHERE user_id=v_user AND name='[TEST-S185] Basic Playlist';
  SELECT id INTO v_ordered FROM playlists WHERE user_id=v_user AND name='[TEST-S185] Ordered Playlist';
  -- Never silently overwrite altered fixture membership: fail instead.
  SELECT COUNT(*) INTO v_conflicts FROM playlist_tracks pt
  WHERE (pt.playlist_id=v_empty)
     OR (pt.playlist_id=v_basic AND NOT ((pt.track_id=1 AND pt.position=1) OR (pt.track_id=2 AND pt.position=2) OR (pt.track_id=3 AND pt.position=3)))
     OR (pt.playlist_id=v_ordered AND NOT ((pt.track_id=4 AND pt.position=1) OR (pt.track_id=3 AND pt.position=2) OR (pt.track_id=2 AND pt.position=3) OR (pt.track_id=1 AND pt.position=4)));
  IF v_conflicts <> 0 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='SCRUM-185 fixture membership has unexpected data; no changes applied';
  END IF;
  INSERT INTO playlist_tracks (playlist_id,track_id,position)
  SELECT v_basic,t.track_id,t.position FROM
    (SELECT 1 AS track_id,1 AS position UNION ALL SELECT 2,2 UNION ALL SELECT 3,3) t
  WHERE NOT EXISTS (SELECT 1 FROM playlist_tracks pt WHERE pt.playlist_id=v_basic AND pt.track_id=t.track_id AND pt.position=t.position);
  INSERT INTO playlist_tracks (playlist_id,track_id,position)
  SELECT v_ordered,t.track_id,t.position FROM
    (SELECT 4 AS track_id,1 AS position UNION ALL SELECT 3,2 UNION ALL SELECT 2,3 UNION ALL SELECT 1,4) t
  WHERE NOT EXISTS (SELECT 1 FROM playlist_tracks pt WHERE pt.playlist_id=v_ordered AND pt.track_id=t.track_id AND pt.position=t.position);
  COMMIT;
END$$
DELIMITER ;
CALL scrum185_install_fixtures();
DROP PROCEDURE scrum185_install_fixtures;
