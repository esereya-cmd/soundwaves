-- ============================================================
-- Soundwave Development Seed Data
-- File: database/init/002_dev_seed.sql
--
-- Purpose:
-- Populate the development database with representative
-- sample artists, albums, and tracks.
--
-- NOTE:
-- Album release years are represented as January 1 dates because
-- the schema requires release_date; exact dates were not provided.
-- Track durations are converted from seconds to milliseconds.
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
    release_date,
    artwork_path
)
VALUES
    (
        1,
        1,
        'Late Nights',
        '2026-01-01',
        '/artwork/late-nights.jpg'
    ),
    (
        2,
        2,
        'Coastal Signals',
        '2025-01-01',
        '/artwork/coastal-signals.jpg'
    ),
    (
        3,
        3,
        'Digital Dreams',
        '2026-01-01',
        '/artwork/digital-dreams.jpg'
    ),
    (
        4,
        4,
        'Static Summer',
        '2024-01-01',
        '/artwork/static-summer.jpg'
    ),
    (
        5,
        5,
        'Open Roads',
        '2025-01-01',
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
    duration_ms,
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
        222000,
        '/music/nova-lane/late-nights/01-midnight-drive.mp3'
    ),
    (
        2,
        1,
        1,
        'Neon Rain',
        2,
        198000,
        '/music/nova-lane/late-nights/02-neon-rain.mp3'
    ),
    (
        3,
        1,
        1,
        'City Lights',
        3,
        245000,
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
        231000,
        '/music/echo-harbor/coastal-signals/01-ocean-static.mp3'
    ),
    (
        5,
        2,
        2,
        'Lighthouse',
        2,
        214000,
        '/music/echo-harbor/coastal-signals/02-lighthouse.mp3'
    ),
    (
        6,
        2,
        2,
        'After the Tide',
        3,
        267000,
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
        203000,
        '/music/midnight-circuit/digital-dreams/01-electric-heart.mp3'
    ),
    (
        8,
        3,
        3,
        'Digital Dreams',
        2,
        252000,
        '/music/midnight-circuit/digital-dreams/02-digital-dreams.mp3'
    ),
    (
        9,
        3,
        3,
        'Afterimage',
        3,
        219000,
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
        188000,
        '/music/paper-satellites/static-summer/01-polaroid-sky.mp3'
    ),
    (
        11,
        4,
        4,
        'Radio Silence',
        2,
        226000,
        '/music/paper-satellites/static-summer/02-radio-silence.mp3'
    ),
    (
        12,
        4,
        4,
        'August Again',
        3,
        241000,
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
        236000,
        '/music/lena-rivers/open-roads/01-open-roads.mp3'
    ),
    (
        14,
        5,
        5,
        'Golden Hour',
        2,
        205000,
        '/music/lena-rivers/open-roads/02-golden-hour.mp3'
    ),
    (
        15,
        5,
        5,
        'Home Again',
        3,
        258000,
        '/music/lena-rivers/open-roads/03-home-again.mp3'
    );

-- ============================================================
-- SPRINT 3 CATALOG EXPANSION (SCRUM-96)
-- Additional fictional metadata for Browse, Details, and Search tests.
-- Apply this file once to a fresh schema; IDs 1-5 / 1-5 / 1-15
-- are reserved for the original artists / albums / tracks.
-- Audio and artwork paths are placeholders, not real media files.
-- ============================================================

-- Additional artists (IDs 6-20)
INSERT INTO artists (id, name) VALUES
    (6, 'Amber Atlas'),
    (7, 'Velvet Horizon'),
    (8, 'Orion Bloom'),
    (9, 'Silver Current'),
    (10, 'Juniper Vale'),
    (11, 'Glass Avenue'),
    (12, 'Cedar Echo'),
    (13, 'Polar Cinema'),
    (14, 'Mira Solstice'),
    (15, 'The Quiet Engines'),
    (16, 'Blue Meridian'),
    (17, 'Saffron Skies'),
    (18, 'River Arcade'),
    (19, 'Cloud District'),
    (20, 'Solstice Parade');

-- Additional albums (IDs 6-40)
INSERT INTO albums (id, artist_id, title, release_date, artwork_path) VALUES
    (6, 6, 'Starlit Maps', '2023-01-01', '/artwork/starlit-maps.jpg'),
    (7, 6, 'Northern Glow', '2025-01-01', '/artwork/northern-glow.jpg'),
    (8, 7, 'Velvet Hours', '2024-01-01', '/artwork/velvet-hours.jpg'),
    (9, 7, 'Soft Focus', '2026-01-01', '/artwork/soft-focus.jpg'),
    (10, 8, 'Orbiting Home', '2022-01-01', '/artwork/orbiting-home.jpg'),
    (11, 8, 'Wildflower Signals', '2025-01-01', '/artwork/wildflower-signals.jpg'),
    (12, 9, 'Silver Tides', '2023-01-01', '/artwork/silver-tides.jpg'),
    (13, 9, 'Underwater Cities', '2026-01-01', '/artwork/underwater-cities.jpg'),
    (14, 10, 'Quiet Mountains', '2021-01-01', '/artwork/quiet-mountains.jpg'),
    (15, 10, 'Autumn Letters', '2024-01-01', '/artwork/autumn-letters.jpg'),
    (16, 11, 'Glass Gardens', '2022-01-01', '/artwork/glass-gardens.jpg'),
    (17, 11, 'After the Rain', '2025-01-01', '/artwork/after-the-rain.jpg'),
    (18, 12, 'Cedar Trails', '2023-01-01', '/artwork/cedar-trails.jpg'),
    (19, 12, 'Firefly Season', '2026-01-01', '/artwork/firefly-season.jpg'),
    (20, 13, 'Frozen Frames', '2021-01-01', '/artwork/frozen-frames.jpg'),
    (21, 13, 'Northern Lights', '2025-01-01', '/artwork/northern-lights.jpg'),
    (22, 14, 'Sunroom Stories', '2022-01-01', '/artwork/sunroom-stories.jpg'),
    (23, 14, 'Golden Echoes', '2024-01-01', '/artwork/golden-echoes.jpg'),
    (24, 15, 'Clockwork Heart', '2023-01-01', '/artwork/clockwork-heart.jpg'),
    (25, 15, 'Paper Machines', '2025-01-01', '/artwork/paper-machines.jpg'),
    (26, 16, 'Blue Horizon', '2021-01-01', '/artwork/blue-horizon.jpg'),
    (27, 16, 'Deep Blue', '2024-01-01', '/artwork/deep-blue.jpg'),
    (28, 17, 'Saffron Morning', '2023-01-01', '/artwork/saffron-morning.jpg'),
    (29, 17, 'Sunset Market', '2026-01-01', '/artwork/sunset-market.jpg'),
    (30, 18, 'River Songs', '2022-01-01', '/artwork/river-songs.jpg'),
    (31, 18, 'Midnight Arcade', '2025-01-01', '/artwork/midnight-arcade.jpg'),
    (32, 19, 'Cloud Nine', '2023-01-01', '/artwork/cloud-nine.jpg'),
    (33, 19, 'Skyline Letters', '2026-01-01', '/artwork/skyline-letters.jpg'),
    (34, 20, 'Parade of Stars', '2022-01-01', '/artwork/parade-of-stars.jpg'),
    (35, 20, 'Last Light', '2025-01-01', '/artwork/last-light.jpg'),
    (36, 6, 'City After Dark', '2026-01-01', '/artwork/city-after-dark.jpg'),
    (37, 9, 'Falling Into Focus', '2024-01-01', '/artwork/falling-into-focus.jpg'),
    (38, 12, 'Acoustic Windows', '2025-01-01', '/artwork/acoustic-windows.jpg'),
    (39, 15, 'Satellite Dreams', '2026-01-01', '/artwork/satellite-dreams.jpg'),
    (40, 18, 'Beyond the Shore', '2026-01-01', '/artwork/beyond-the-shore.jpg');

-- Additional tracks (IDs 16-120), three tracks per new album.
-- Track names deliberately repeat some search terms across artists.
INSERT INTO tracks (id, album_id, artist_id, title, track_number, duration_ms, file_path, mime_type) VALUES
    (16, 6, 6, 'First Light', 1, 194000, '/music/amber-atlas/starlit-maps/01-first-light.mp3', 'audio/mpeg'),
    (17, 6, 6, 'Midnight Signal', 2, 223000, '/music/amber-atlas/starlit-maps/02-midnight-signal.mp3', 'audio/mpeg'),
    (18, 6, 6, 'Homeward Bound', 3, 252000, '/music/amber-atlas/starlit-maps/03-homeward-bound.mp3', 'audio/mpeg'),
    (19, 7, 6, 'Echoes in Blue', 1, 211000, '/music/amber-atlas/northern-glow/01-echoes-in-blue.mp3', 'audio/mpeg'),
    (20, 7, 6, 'Open Window', 2, 240000, '/music/amber-atlas/northern-glow/02-open-window.mp3', 'audio/mpeg'),
    (21, 7, 6, 'Summer Rain', 3, 269000, '/music/amber-atlas/northern-glow/03-summer-rain.mp3', 'audio/mpeg'),
    (22, 8, 7, 'Lost in Motion', 1, 228000, '/music/velvet-horizon/velvet-hours/01-lost-in-motion.mp3', 'audio/mpeg'),
    (23, 8, 7, 'Quiet Streets', 2, 257000, '/music/velvet-horizon/velvet-hours/02-quiet-streets.mp3', 'audio/mpeg'),
    (24, 8, 7, 'Another Sunrise', 3, 286000, '/music/velvet-horizon/velvet-hours/03-another-sunrise.mp3', 'audio/mpeg'),
    (25, 9, 7, 'Silver Lining', 1, 245000, '/music/velvet-horizon/soft-focus/01-silver-lining.mp3', 'audio/mpeg'),
    (26, 9, 7, 'After Hours', 2, 274000, '/music/velvet-horizon/soft-focus/02-after-hours.mp3', 'audio/mpeg'),
    (27, 9, 7, 'Long Way Home', 3, 173000, '/music/velvet-horizon/soft-focus/03-long-way-home.mp3', 'audio/mpeg'),
    (28, 10, 8, 'Golden Days', 1, 262000, '/music/orion-bloom/orbiting-home/01-golden-days.mp3', 'audio/mpeg'),
    (29, 10, 8, 'City of Stars', 2, 291000, '/music/orion-bloom/orbiting-home/02-city-of-stars.mp3', 'audio/mpeg'),
    (30, 10, 8, 'Northern Wind', 3, 190000, '/music/orion-bloom/orbiting-home/03-northern-wind.mp3', 'audio/mpeg'),
    (31, 11, 8, 'Parallel Lines', 1, 279000, '/music/orion-bloom/wildflower-signals/01-parallel-lines.mp3', 'audio/mpeg'),
    (32, 11, 8, 'Paper Moon', 2, 178000, '/music/orion-bloom/wildflower-signals/02-paper-moon.mp3', 'audio/mpeg'),
    (33, 11, 8, 'Distant Voices', 3, 207000, '/music/orion-bloom/wildflower-signals/03-distant-voices.mp3', 'audio/mpeg'),
    (34, 12, 9, 'Slow Motion', 1, 166000, '/music/silver-current/silver-tides/01-slow-motion.mp3', 'audio/mpeg'),
    (35, 12, 9, 'New Horizons', 2, 195000, '/music/silver-current/silver-tides/02-new-horizons.mp3', 'audio/mpeg'),
    (36, 12, 9, 'Fading Polaroids', 3, 224000, '/music/silver-current/silver-tides/03-fading-polaroids.mp3', 'audio/mpeg'),
    (37, 13, 9, 'Electric Skies', 1, 183000, '/music/silver-current/underwater-cities/01-electric-skies.mp3', 'audio/mpeg'),
    (38, 13, 9, 'Ocean Avenue', 2, 212000, '/music/silver-current/underwater-cities/02-ocean-avenue.mp3', 'audio/mpeg'),
    (39, 13, 9, 'Still Here', 3, 241000, '/music/silver-current/underwater-cities/03-still-here.mp3', 'audio/mpeg'),
    (40, 14, 10, 'Night Drive', 1, 200000, '/music/juniper-vale/quiet-mountains/01-night-drive.mp3', 'audio/mpeg'),
    (41, 14, 10, 'Dream in Color', 2, 229000, '/music/juniper-vale/quiet-mountains/02-dream-in-color.mp3', 'audio/mpeg'),
    (42, 14, 10, 'Crossing Paths', 3, 258000, '/music/juniper-vale/quiet-mountains/03-crossing-paths.mp3', 'audio/mpeg'),
    (43, 15, 10, 'Satellite Heart', 1, 217000, '/music/juniper-vale/autumn-letters/01-satellite-heart.mp3', 'audio/mpeg'),
    (44, 15, 10, 'Morning Tide', 2, 246000, '/music/juniper-vale/autumn-letters/02-morning-tide.mp3', 'audio/mpeg'),
    (45, 15, 10, 'Better Days', 3, 275000, '/music/juniper-vale/autumn-letters/03-better-days.mp3', 'audio/mpeg'),
    (46, 16, 11, 'Lights Out', 1, 234000, '/music/glass-avenue/glass-gardens/01-lights-out.mp3', 'audio/mpeg'),
    (47, 16, 11, 'Soft Landing', 2, 263000, '/music/glass-avenue/glass-gardens/02-soft-landing.mp3', 'audio/mpeg'),
    (48, 16, 11, 'Blue Room', 3, 292000, '/music/glass-avenue/glass-gardens/03-blue-room.mp3', 'audio/mpeg'),
    (49, 17, 11, 'Hold the Line', 1, 251000, '/music/glass-avenue/after-the-rain/01-hold-the-line.mp3', 'audio/mpeg'),
    (50, 17, 11, 'Endless Summer', 2, 280000, '/music/glass-avenue/after-the-rain/02-endless-summer.mp3', 'audio/mpeg'),
    (51, 17, 11, 'First Light', 3, 179000, '/music/glass-avenue/after-the-rain/03-first-light.mp3', 'audio/mpeg'),
    (52, 18, 12, 'Midnight Signal', 1, 268000, '/music/cedar-echo/cedar-trails/01-midnight-signal.mp3', 'audio/mpeg'),
    (53, 18, 12, 'Homeward Bound', 2, 167000, '/music/cedar-echo/cedar-trails/02-homeward-bound.mp3', 'audio/mpeg'),
    (54, 18, 12, 'Echoes in Blue', 3, 196000, '/music/cedar-echo/cedar-trails/03-echoes-in-blue.mp3', 'audio/mpeg'),
    (55, 19, 12, 'Open Window', 1, 285000, '/music/cedar-echo/firefly-season/01-open-window.mp3', 'audio/mpeg'),
    (56, 19, 12, 'Summer Rain', 2, 184000, '/music/cedar-echo/firefly-season/02-summer-rain.mp3', 'audio/mpeg'),
    (57, 19, 12, 'Lost in Motion', 3, 213000, '/music/cedar-echo/firefly-season/03-lost-in-motion.mp3', 'audio/mpeg'),
    (58, 20, 13, 'Quiet Streets', 1, 172000, '/music/polar-cinema/frozen-frames/01-quiet-streets.mp3', 'audio/mpeg'),
    (59, 20, 13, 'Another Sunrise', 2, 201000, '/music/polar-cinema/frozen-frames/02-another-sunrise.mp3', 'audio/mpeg'),
    (60, 20, 13, 'Silver Lining', 3, 230000, '/music/polar-cinema/frozen-frames/03-silver-lining.mp3', 'audio/mpeg'),
    (61, 21, 13, 'After Hours', 1, 189000, '/music/polar-cinema/northern-lights/01-after-hours.mp3', 'audio/mpeg'),
    (62, 21, 13, 'Long Way Home', 2, 218000, '/music/polar-cinema/northern-lights/02-long-way-home.mp3', 'audio/mpeg'),
    (63, 21, 13, 'Golden Days', 3, 247000, '/music/polar-cinema/northern-lights/03-golden-days.mp3', 'audio/mpeg'),
    (64, 22, 14, 'City of Stars', 1, 206000, '/music/mira-solstice/sunroom-stories/01-city-of-stars.mp3', 'audio/mpeg'),
    (65, 22, 14, 'Northern Wind', 2, 235000, '/music/mira-solstice/sunroom-stories/02-northern-wind.mp3', 'audio/mpeg'),
    (66, 22, 14, 'Parallel Lines', 3, 264000, '/music/mira-solstice/sunroom-stories/03-parallel-lines.mp3', 'audio/mpeg'),
    (67, 23, 14, 'Paper Moon', 1, 223000, '/music/mira-solstice/golden-echoes/01-paper-moon.mp3', 'audio/mpeg'),
    (68, 23, 14, 'Distant Voices', 2, 252000, '/music/mira-solstice/golden-echoes/02-distant-voices.mp3', 'audio/mpeg'),
    (69, 23, 14, 'Slow Motion', 3, 281000, '/music/mira-solstice/golden-echoes/03-slow-motion.mp3', 'audio/mpeg'),
    (70, 24, 15, 'New Horizons', 1, 240000, '/music/the-quiet-engines/clockwork-heart/01-new-horizons.mp3', 'audio/mpeg'),
    (71, 24, 15, 'Fading Polaroids', 2, 269000, '/music/the-quiet-engines/clockwork-heart/02-fading-polaroids.mp3', 'audio/mpeg'),
    (72, 24, 15, 'Electric Skies', 3, 168000, '/music/the-quiet-engines/clockwork-heart/03-electric-skies.mp3', 'audio/mpeg'),
    (73, 25, 15, 'Ocean Avenue', 1, 257000, '/music/the-quiet-engines/paper-machines/01-ocean-avenue.mp3', 'audio/mpeg'),
    (74, 25, 15, 'Still Here', 2, 286000, '/music/the-quiet-engines/paper-machines/02-still-here.mp3', 'audio/mpeg'),
    (75, 25, 15, 'Night Drive', 3, 185000, '/music/the-quiet-engines/paper-machines/03-night-drive.mp3', 'audio/mpeg'),
    (76, 26, 16, 'Dream in Color', 1, 274000, '/music/blue-meridian/blue-horizon/01-dream-in-color.mp3', 'audio/mpeg'),
    (77, 26, 16, 'Crossing Paths', 2, 173000, '/music/blue-meridian/blue-horizon/02-crossing-paths.mp3', 'audio/mpeg'),
    (78, 26, 16, 'Satellite Heart', 3, 202000, '/music/blue-meridian/blue-horizon/03-satellite-heart.mp3', 'audio/mpeg'),
    (79, 27, 16, 'Morning Tide', 1, 291000, '/music/blue-meridian/deep-blue/01-morning-tide.mp3', 'audio/mpeg'),
    (80, 27, 16, 'Better Days', 2, 190000, '/music/blue-meridian/deep-blue/02-better-days.mp3', 'audio/mpeg'),
    (81, 27, 16, 'Lights Out', 3, 219000, '/music/blue-meridian/deep-blue/03-lights-out.mp3', 'audio/mpeg'),
    (82, 28, 17, 'Soft Landing', 1, 178000, '/music/saffron-skies/saffron-morning/01-soft-landing.mp3', 'audio/mpeg'),
    (83, 28, 17, 'Blue Room', 2, 207000, '/music/saffron-skies/saffron-morning/02-blue-room.mp3', 'audio/mpeg'),
    (84, 28, 17, 'Hold the Line', 3, 236000, '/music/saffron-skies/saffron-morning/03-hold-the-line.mp3', 'audio/mpeg'),
    (85, 29, 17, 'Endless Summer', 1, 195000, '/music/saffron-skies/sunset-market/01-endless-summer.mp3', 'audio/mpeg'),
    (86, 29, 17, 'First Light', 2, 224000, '/music/saffron-skies/sunset-market/02-first-light.mp3', 'audio/mpeg'),
    (87, 29, 17, 'Midnight Signal', 3, 253000, '/music/saffron-skies/sunset-market/03-midnight-signal.mp3', 'audio/mpeg'),
    (88, 30, 18, 'Homeward Bound', 1, 212000, '/music/river-arcade/river-songs/01-homeward-bound.mp3', 'audio/mpeg'),
    (89, 30, 18, 'Echoes in Blue', 2, 241000, '/music/river-arcade/river-songs/02-echoes-in-blue.mp3', 'audio/mpeg'),
    (90, 30, 18, 'Open Window', 3, 270000, '/music/river-arcade/river-songs/03-open-window.mp3', 'audio/mpeg'),
    (91, 31, 18, 'Summer Rain', 1, 229000, '/music/river-arcade/midnight-arcade/01-summer-rain.mp3', 'audio/mpeg'),
    (92, 31, 18, 'Lost in Motion', 2, 258000, '/music/river-arcade/midnight-arcade/02-lost-in-motion.mp3', 'audio/mpeg'),
    (93, 31, 18, 'Quiet Streets', 3, 287000, '/music/river-arcade/midnight-arcade/03-quiet-streets.mp3', 'audio/mpeg'),
    (94, 32, 19, 'Another Sunrise', 1, 246000, '/music/cloud-district/cloud-nine/01-another-sunrise.mp3', 'audio/mpeg'),
    (95, 32, 19, 'Silver Lining', 2, 275000, '/music/cloud-district/cloud-nine/02-silver-lining.mp3', 'audio/mpeg'),
    (96, 32, 19, 'After Hours', 3, 174000, '/music/cloud-district/cloud-nine/03-after-hours.mp3', 'audio/mpeg'),
    (97, 33, 19, 'Long Way Home', 1, 263000, '/music/cloud-district/skyline-letters/01-long-way-home.mp3', 'audio/mpeg'),
    (98, 33, 19, 'Golden Days', 2, 292000, '/music/cloud-district/skyline-letters/02-golden-days.mp3', 'audio/mpeg'),
    (99, 33, 19, 'City of Stars', 3, 191000, '/music/cloud-district/skyline-letters/03-city-of-stars.mp3', 'audio/mpeg'),
    (100, 34, 20, 'Northern Wind', 1, 280000, '/music/solstice-parade/parade-of-stars/01-northern-wind.mp3', 'audio/mpeg'),
    (101, 34, 20, 'Parallel Lines', 2, 179000, '/music/solstice-parade/parade-of-stars/02-parallel-lines.mp3', 'audio/mpeg'),
    (102, 34, 20, 'Paper Moon', 3, 208000, '/music/solstice-parade/parade-of-stars/03-paper-moon.mp3', 'audio/mpeg'),
    (103, 35, 20, 'Distant Voices', 1, 167000, '/music/solstice-parade/last-light/01-distant-voices.mp3', 'audio/mpeg'),
    (104, 35, 20, 'Slow Motion', 2, 196000, '/music/solstice-parade/last-light/02-slow-motion.mp3', 'audio/mpeg'),
    (105, 35, 20, 'New Horizons', 3, 225000, '/music/solstice-parade/last-light/03-new-horizons.mp3', 'audio/mpeg'),
    (106, 36, 6, 'Fading Polaroids', 1, 184000, '/music/amber-atlas/city-after-dark/01-fading-polaroids.mp3', 'audio/mpeg'),
    (107, 36, 6, 'Electric Skies', 2, 213000, '/music/amber-atlas/city-after-dark/02-electric-skies.mp3', 'audio/mpeg'),
    (108, 36, 6, 'Ocean Avenue', 3, 242000, '/music/amber-atlas/city-after-dark/03-ocean-avenue.mp3', 'audio/mpeg'),
    (109, 37, 9, 'Still Here', 1, 201000, '/music/silver-current/falling-into-focus/01-still-here.mp3', 'audio/mpeg'),
    (110, 37, 9, 'Night Drive', 2, 230000, '/music/silver-current/falling-into-focus/02-night-drive.mp3', 'audio/mpeg'),
    (111, 37, 9, 'Dream in Color', 3, 259000, '/music/silver-current/falling-into-focus/03-dream-in-color.mp3', 'audio/mpeg'),
    (112, 38, 12, 'Crossing Paths', 1, 218000, '/music/cedar-echo/acoustic-windows/01-crossing-paths.mp3', 'audio/mpeg'),
    (113, 38, 12, 'Satellite Heart', 2, 247000, '/music/cedar-echo/acoustic-windows/02-satellite-heart.mp3', 'audio/mpeg'),
    (114, 38, 12, 'Morning Tide', 3, 276000, '/music/cedar-echo/acoustic-windows/03-morning-tide.mp3', 'audio/mpeg'),
    (115, 39, 15, 'Better Days', 1, 235000, '/music/the-quiet-engines/satellite-dreams/01-better-days.mp3', 'audio/mpeg'),
    (116, 39, 15, 'Lights Out', 2, 264000, '/music/the-quiet-engines/satellite-dreams/02-lights-out.mp3', 'audio/mpeg'),
    (117, 39, 15, 'Soft Landing', 3, 293000, '/music/the-quiet-engines/satellite-dreams/03-soft-landing.mp3', 'audio/mpeg'),
    (118, 40, 18, 'Blue Room', 1, 252000, '/music/river-arcade/beyond-the-shore/01-blue-room.mp3', 'audio/mpeg'),
    (119, 40, 18, 'Hold the Line', 2, 281000, '/music/river-arcade/beyond-the-shore/02-hold-the-line.mp3', 'audio/mpeg'),
    (120, 40, 18, 'Endless Summer', 3, 180000, '/music/river-arcade/beyond-the-shore/03-endless-summer.mp3', 'audio/mpeg');

-- Verification queries (read-only):
-- SELECT COUNT(*) FROM artists; -- expected: 20
-- SELECT COUNT(*) FROM albums;  -- expected: 40
-- SELECT COUNT(*) FROM tracks;  -- expected: 120
-- SELECT COUNT(*) FROM albums a LEFT JOIN artists ar ON a.artist_id = ar.id WHERE a.artist_id IS NOT NULL AND ar.id IS NULL; -- 0
-- SELECT COUNT(*) FROM tracks t LEFT JOIN albums a ON t.album_id = a.id WHERE t.album_id IS NOT NULL AND a.id IS NULL; -- 0
-- SELECT COUNT(*) FROM tracks t JOIN albums a ON t.album_id = a.id WHERE t.artist_id <> a.artist_id; -- 0
