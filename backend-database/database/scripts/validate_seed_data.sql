
USE soundwave;

-- Check if the demo artist exists
SELECT COUNT(*) AS demo_artist_found
FROM artists
WHERE name = 'Demo Artist';

-- Check if the demo album exists
SELECT COUNT(*) AS demo_album_found
FROM albums a
JOIN artists ar ON ar.id = a.artist_id
WHERE a.title = 'Demo Album'
  AND ar.name = 'Demo Artist';

-- Check if the demo track exists
SELECT COUNT(*) AS demo_track_found
FROM tracks t
JOIN albums a ON a.id = t.album_id
JOIN artists ar ON ar.id = t.artist_id
WHERE t.title = 'Demo Track'
  AND a.title = 'Demo Album'
  AND ar.name = 'Demo Artist';

-- SCRUM-99: Automated seed data validation

-- Remove an old procedure if one already exists
DROP PROCEDURE IF EXISTS validate_demo_seed;

DELIMITER //

CREATE PROCEDURE validate_demo_seed()
BEGIN
    DECLARE artist_count INT DEFAULT 0;
    DECLARE album_count INT DEFAULT 0;
    DECLARE track_count INT DEFAULT 0;

    -- Count demo artists
    SELECT COUNT(*) INTO artist_count
    FROM artists
    WHERE name = 'Demo Artist';

    -- Count demo albums linked to the demo artist
    SELECT COUNT(*) INTO album_count
    FROM albums a
    JOIN artists ar ON ar.id = a.artist_id
    WHERE a.title = 'Demo Album'
      AND ar.name = 'Demo Artist';

    -- Count demo tracks linked to the correct album and artist
    SELECT COUNT(*) INTO track_count
    FROM tracks t
    JOIN albums a ON a.id = t.album_id
    JOIN artists ar ON ar.id = t.artist_id
    WHERE t.title = 'Demo Track'
      AND a.title = 'Demo Album'
      AND ar.name = 'Demo Artist';

    -- Fail validation if records are missing or duplicated
    IF artist_count <> 1 OR album_count <> 1 OR track_count <> 1 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Seed validation failed: missing or duplicate demo data';
    END IF;

    -- Display success if all checks pass
    SELECT 'PASS: demo seed data validated' AS validation_result;

END//

DELIMITER ;

-- Execute validation
CALL validate_demo_seed();

-- Clean up the procedure after successful validation
DROP PROCEDURE IF EXISTS validate_demo_seed;
