-- ============================================================
-- Soundwave Development Seed Data
-- File: database/seeds/001_dev_seed.sql
--
-- Purpose:
-- Populate the development database with representative
-- sample artists, albums, and tracks.
--
-- NOTE:
-- All artists, albums, and songs below are fictional and
-- intended only for development/testing.
-- ============================================================

USE soundwave;

-- ============================================================
-- ARTISTS
-- ============================================================

INSERT INTO artists (id, name)
VALUES
    (1, 'Nova Lane'),
    (2, 'Echo Harbor'),
    (3, 'Midnight Circuit'),
    (4, 'The Paper Satellites'),
    (5, 'Lena Rivers');


-- ============================================================
-- ALBUMS
-- ============================================================

INSERT INTO albums (
    id,
    artist_id,
    title,
    release_year,
    artwork_url
)
VALUES
    (
        1,
        1,
        'Late Nights',
        2026,
        '/artwork/late-nights.jpg'
    ),
    (
        2,
        2,
        'Coastal Signals',
        2025,
        '/artwork/coastal-signals.jpg'
    ),
    (
        3,
        3,
        'Digital Dreams',
        2026,
        '/artwork/digital-dreams.jpg'
    ),
    (
        4,
        4,
        'Static Summer',
        2024,
        '/artwork/static-summer.jpg'
    ),
    (
        5,
        5,
        'Open Roads',
        2025,
        '/artwork/open-roads.jpg'
    );


-- ============================================================
-- TRACKS / SONGS
-- ============================================================

INSERT INTO tracks (
    id,
    album_id,
    artist_id,
    title,
    track_number,
    duration_seconds,
    file_path
)
VALUES

    -- --------------------------------------------------------
    -- Nova Lane - Late Nights
    -- --------------------------------------------------------

    (
        1,
        1,
        1,
        'Midnight Drive',
        1,
        222,
        '/music/nova-lane/late-nights/01-midnight-drive.mp3'
    ),
    (
        2,
        1,
        1,
        'Neon Rain',
        2,
        198,
        '/music/nova-lane/late-nights/02-neon-rain.mp3'
    ),
    (
        3,
        1,
        1,
        'City Lights',
        3,
        245,
        '/music/nova-lane/late-nights/03-city-lights.mp3'
    ),

    -- --------------------------------------------------------
    -- Echo Harbor - Coastal Signals
    -- --------------------------------------------------------

    (
        4,
        2,
        2,
        'Ocean Static',
        1,
        231,
        '/music/echo-harbor/coastal-signals/01-ocean-static.mp3'
    ),
    (
        5,
        2,
        2,
        'Lighthouse',
        2,
        214,
        '/music/echo-harbor/coastal-signals/02-lighthouse.mp3'
    ),
    (
        6,
        2,
        2,
        'After the Tide',
        3,
        267,
        '/music/echo-harbor/coastal-signals/03-after-the-tide.mp3'
    ),

    -- --------------------------------------------------------
    -- Midnight Circuit - Digital Dreams
    -- --------------------------------------------------------

    (
        7,
        3,
        3,
        'Electric Heart',
        1,
        203,
        '/music/midnight-circuit/digital-dreams/01-electric-heart.mp3'
    ),
    (
        8,
        3,
        3,
        'Digital Dreams',
        2,
        252,
        '/music/midnight-circuit/digital-dreams/02-digital-dreams.mp3'
    ),
    (
        9,
        3,
        3,
        'Afterimage',
        3,
        219,
        '/music/midnight-circuit/digital-dreams/03-afterimage.mp3'
    ),

    -- --------------------------------------------------------
    -- The Paper Satellites - Static Summer
    -- --------------------------------------------------------

    (
        10,
        4,
        4,
        'Polaroid Sky',
        1,
        188,
        '/music/paper-satellites/static-summer/01-polaroid-sky.mp3'
    ),
    (
        11,
        4,
        4,
        'Radio Silence',
        2,
        226,
        '/music/paper-satellites/static-summer/02-radio-silence.mp3'
    ),
    (
        12,
        4,
        4,
        'August Again',
        3,
        241,
        '/music/paper-satellites/static-summer/03-august-again.mp3'
    ),

    -- --------------------------------------------------------
    -- Lena Rivers - Open Roads
    -- --------------------------------------------------------

    (
        13,
        5,
        5,
        'Open Roads',
        1,
        236,
        '/music/lena-rivers/open-roads/01-open-roads.mp3'
    ),
    (
        14,
        5,
        5,
        'Golden Hour',
        2,
        205,
        '/music/lena-rivers/open-roads/02-golden-hour.mp3'
    ),
    (
        15,
        5,
        5,
        'Home Again',
        3,
        258,
        '/music/lena-rivers/open-roads/03-home-again.mp3'
    );
